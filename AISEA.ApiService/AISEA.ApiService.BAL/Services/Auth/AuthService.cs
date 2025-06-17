using System.Security.Authentication;
using System.Text.Json;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Auth;
using AISEA.ApiService.SHARED.DTOs.Responses.Auth;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AutoMapper;
using Google.Apis.Auth;

namespace AISEA.ApiService.BAL.Services.Auth;

public class AuthService
{
    private readonly GoogleAuthSettings _googleAuthSettings;
    private readonly HttpClient _httpClient;
    private readonly UserRepository _userRepository;
    private readonly IJWTService _jwtService;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(GoogleAuthSettings googleAuthSettings, IHttpClientFactory httpClientFactory, UserRepository userRepository, IJWTService jwtService, ITokenService tokenService, IMapper mapper)
    {
        _googleAuthSettings = googleAuthSettings;
        _httpClient = httpClientFactory.CreateClient();
        _userRepository = userRepository;
        _jwtService = jwtService;
        _tokenService = tokenService;
        _mapper = mapper;
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
        throw new NotImplementedException();
    }
}