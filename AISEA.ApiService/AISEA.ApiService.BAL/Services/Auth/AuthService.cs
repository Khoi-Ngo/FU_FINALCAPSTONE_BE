using System.Security.Authentication;
using System.Text.Json;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Auth;
using AISEA.ApiService.SHARED.DTOs.Responses.Auth;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using BC = BCrypt.Net.BCrypt;

namespace AISEA.ApiService.BAL.Services.Auth;

public class AuthService
{
    private readonly GoogleAuthSettings _googleAuthSettings;
    private readonly HttpClient _httpClient;
    private readonly UserRepository _userRepository;
    private readonly IJWTService _jwtService;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly EndpointSettings _endpointSettings;
    private readonly IMailService _mailService;
    private readonly IRedisRepository _redisRepository;
    private readonly VerifyResetPassCodeSettings _verifyResetPassCodeSettings;

    public AuthService(GoogleAuthSettings googleAuthSettings, IHttpClientFactory httpClientFactory, UserRepository userRepository, IJWTService jwtService, ITokenService tokenService, IMapper mapper, EndpointSettings endpointSettings, IMailService mailService, IRedisRepository redisRepository, VerifyResetPassCodeSettings verifyResetPassCodeSettings)
    {
        _googleAuthSettings = googleAuthSettings;
        _httpClient = httpClientFactory.CreateClient();
        _userRepository = userRepository;
        _jwtService = jwtService;
        _tokenService = tokenService;
        _mapper = mapper;
        _endpointSettings = endpointSettings;
        _mailService = mailService;
        _redisRepository = redisRepository;
        _verifyResetPassCodeSettings = verifyResetPassCodeSettings;
    }

    public async Task ForgetPasswordAsync(ForgetPasswordFEIDRequest request)
    {
        //verify email and verification code are all valid
        var isValidVerifyResetCode = await _redisRepository.IsValidVerifyResetCodeAsync(request.Email, request.VerificationCode);
        if (!isValidVerifyResetCode)
        {
            throw new InvalidCredentialException("Invalid verification code or email.");
        }

        //remove the verification code from Redis
        await _redisRepository.RemoveVerifyResetCodeAsync(request.Email);

        //reset the new password
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user is null)
        {
            throw new NotFoundException("User with this email does not exist.");
        }
        // Hash and set new password
        user.Password = BC.EnhancedHashPassword(request.NewPassword);
        await _userRepository.UpdateAsync(user);
    }

    public async Task<AuthResponse> GoogleLoginAsync(string token)
    {
        // get user info from Google
        var request = new HttpRequestMessage(HttpMethod.Get, _googleAuthSettings.UserInfoUrl);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            throw new InvalidCGoogleTokenException("Invalid Google access token.");

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;

        var email = root.GetProperty(_googleAuthSettings.UserMailRespPropName).GetString();

        if (string.IsNullOrEmpty(email))
            throw new InvalidCGoogleTokenException("Email not found in token.");

        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user is null)
            throw new InvalidCredentialException("User not found with this email.");

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user.Username);
        var storedToken = await _tokenService.GetRefreshTokenAsync(user.Username);
        var refreshToken = string.IsNullOrEmpty(storedToken) ? _tokenService.GenerateRefreshToken() : storedToken;

        var result = _mapper.Map<AuthResponse>(user);
        result.AccessToken = accessToken;
        result.RefreshToken = refreshToken;
        return result;
    }

    public async Task<AuthResponse> LoginAsync(AuthFEIDRequest request)
    {
        // Validate user credentials
        var user = await _userRepository.GetUserByUsernameAsync(request.Username);
        if (user is null || !BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.Password))
        {
            throw new InvalidCredentialException("Invalid username or password.");
        }

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user.Username);
        var refreshToken = _tokenService.GenerateRefreshToken();

        //saving the refresh token to Redis
        await _tokenService.StoreRefreshTokenAsync(user.Username, refreshToken);

        var result = _mapper.Map<AuthResponse>(user);
        result.AccessToken = accessToken;
        result.RefreshToken = refreshToken;
        return result;
    }

    public async Task LogoutAsync(string accessToken)
    {
        //simply adding the access token to the blacklist
        await _tokenService.BlacklistAccessTokenAsync(accessToken);
    }

    public async Task<RefreshTokenResponse> RefreshAsync(string accessToken, string refreshToken)
    {
        //get the user info from expired access token
        var principal = _jwtService.GetPrincipalFromExpiredToken(accessToken);
        string username = _jwtService.GetValueFromPrincipal(principal, _endpointSettings.UserNameClaimName).ToString();

        // Check if refresh token exists in Redis + refresh token belong to the username
        var isValid = await _tokenService.IsValidRefreshTokenAsync(username, refreshToken);
        if (!isValid)
        {
            throw new InvalidRefreshToken("Invalid refresh token, the refresh token may be gone or not matched with request");
        }

        // Generate new tokens
        string newRefreshToken = _tokenService.GenerateRefreshToken();
        string newAccessToken = _jwtService.GenerateAccessToken(username);

        // Update Redis: Adding the new refresh token to overwrite in Redis
        await _tokenService.StoreRefreshTokenAsync(username, newRefreshToken);

        return new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };

    }

    public async Task ResetPasswordAsync(ResetPasswordFEIDRequest request, string accessToken)
    {
        // Get username from access token
        var username = _jwtService.GetUsernameFromToken(accessToken);
        // Get user from DB
        var user = await _userRepository.GetUserByUsernameAsync(username);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }
        // Compare current password with the one in DB
        if (!BC.EnhancedVerify(request.CurrentPassword, user.Password))
        {
            throw new InvalidCredentialException("Current password is incorrect.");
        }
        // Hash and set new password
        user.Password = BC.EnhancedHashPassword(request.NewPassword);
        await _userRepository.UpdateAsync(user);
    }

    public async Task SendResetCodeAsync(GetVerificationCodeRequest request)
    {
        //verify the email exists in the system
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user is null)
        {
            throw new NotFoundException("User with this email does not exist.");
        }
        // Generate a verification code
        var verificationCode = GenerateVerificationCode();
        // using redis repository to store the verification code
        await _redisRepository.SaveVerifyResetCodeAsync(request.Email, verificationCode, TimeSpan.FromMilliseconds(_verifyResetPassCodeSettings.ExpireMilli));
        // Send the verification code via email
        await _mailService.SendEmailAsync(request.Email, _verifyResetPassCodeSettings.Subject, _verifyResetPassCodeSettings.Body.Replace("{code}", verificationCode));

    }
    private string GenerateVerificationCode(int length = 12, string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
    {
        var random = new Random();
        return new string(Enumerable.Repeat(allowedChars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}