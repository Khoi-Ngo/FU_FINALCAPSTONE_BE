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

    // Reset password
    /// <summary>
    /// Resets the password using current FEID password and new password.
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordFEIDRequest request)
    {
        await _authService.ResetPasswordAsync(request, AccessToken);
        return Ok("Password reset successful.");
    }

    /// <summary>
    /// Refreshes the access token and refresh token with expired (not blacklisted) Access Token and Valid Refresh Token.
    /// </summary>
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
    [HttpGet("logout")]
    public async Task<IActionResult> DemoLogoutWithRedis()
    {
        await _authService.LogoutAsync(AccessToken);
        return Ok("Logout successful.");
    }
}