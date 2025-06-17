using AISEA.ApiService.BAL.Services.Auth;
using AISEA.ApiService.SHARED.DTOs.Requests.Auth;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly AuthService _authService;

    public AuthController(EndpointSettings endpointSettings, AuthService authService) : base(endpointSettings)
    {
        _authService = authService;
    }
    /// <summary>
    /// Login using Google authentication.
    /// </summary>
    /// <returns>Returns authentication result using Access Token of GG SSO.</returns>
    // Login with google
    [HttpGet("google")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginWithGoogle()
    {
        var res = await _authService.GoogleLoginAsync(AuthorizationTokenGoogle);
        return Ok(res);
    }

    /// <summary>
    /// Login with username and password. (case FEID)
    /// </summary>
    /// <returns>Returns authentication result for the user.</returns>
    // Login with username and password
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] AuthFEIDRequest request)
    {
        var res = await _authService.LoginAsync(request);
        return Ok(res);
    }

    // Forget password
    // [HttpPost("forget-password")]
    // [AllowAnonymous]
    // public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordFEIDRequest request)
    // {
    //     var res = await _authService.ForgetPasswordAsync(request);
    //     return Ok(res);
    // }

    // // Reset password
    // [HttpPost("reset-password")]
    // [AllowAnonymous]
    // public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordFEIDRequest request)
    // {
    //     var res = await _authService.ResetPasswordAsync(request);
    //     return Ok(res);
    // }

    /// <summary>
    /// Refreshes the access token and refresh token with expired (not blacklisted) Access Token and Valid Refresh Token.
    /// </summary>
    /// <returns>Returns new access and refresh tokens if valid.</returns>
    [HttpGet("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> DemoRefreshTokenWithRedis()
    {
        var res = await _authService.RefreshAsync(AccessToken, RefreshToken);
        return Ok(res);
    }

    /// <summary>
    /// Logs out the current user and blacklists the access token.
    /// </summary>
    /// <returns>Returns a success message upon logout.</returns>
    [HttpGet("logout")]
    public async Task<IActionResult> DemoLogoutWithRedis()
    {
        await _authService.LogoutAsync(AccessToken);
        return Ok("Logout successful.");
    }
}