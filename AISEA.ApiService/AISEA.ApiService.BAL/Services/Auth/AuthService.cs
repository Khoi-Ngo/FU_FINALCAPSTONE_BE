using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Infrastructure;
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
        var payload = await GoogleJsonWebSignature.ValidateAsync(token);
        var email = payload.Email;
        if (string.IsNullOrEmpty(email))
            throw new InvalidCGoogleTokenException("Email not found in token.");

        // Check existing user
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
}