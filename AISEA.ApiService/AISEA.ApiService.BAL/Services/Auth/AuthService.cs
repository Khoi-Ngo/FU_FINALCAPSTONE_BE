using System.Security.Authentication;
using System.Text.Json;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Auth;
using AISEA.ApiService.SHARED.DTOs.Responses.Auth;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.SHARED.Util;
using AutoMapper;
using Google.Apis.Auth;
using Microsoft.Extensions.Logging;

namespace AISEA.ApiService.BAL.Services.Auth;

public class AuthService
{
    private readonly GoogleAuthSettings _googleAuthSettings;
    private readonly HttpClient _httpClient;
    private readonly UserRepository _userRepository;
    private readonly IJWTService _jwtService;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly JwtSettings _jwtSettings;
    private readonly EndpointSettings _endpointSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(GoogleAuthSettings googleAuthSettings, IHttpClientFactory httpClientFactory, UserRepository userRepository, IJWTService jwtService, ITokenService tokenService, IMapper mapper, JwtSettings jwtSettings, EndpointSettings endpointSettings, ILogger<AuthService> logger)
    {
        _googleAuthSettings = googleAuthSettings;
        _httpClient = httpClientFactory.CreateClient();
        _userRepository = userRepository;
        _jwtService = jwtService;
        _tokenService = tokenService;
        _mapper = mapper;
        _jwtSettings = jwtSettings;
        _endpointSettings = endpointSettings;
        _logger = logger;
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
        var refreshToken = _tokenService.GenerateRefreshToken();

        //saving the refresh token to Redis
        await _tokenService.StoreRefreshTokenAsync(user.Username, refreshToken);

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
        var principal = JWTTokenUtil.GetPrincipalFromExpiredToken(accessToken, _jwtSettings.SecretKey);
        string username = JWTTokenUtil.GetValueFromPrincipal(principal, _endpointSettings.UserNameClaimName).ToString();

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
}