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
    /// Refreshes the access token and refresh token with expired (not blacklisted) Access Token and Valid Refresh Token.
    /// </summary>
    [HttpGet("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(
        [FromHeader(Name = "RefreshToken")] string refreshToken)
    {
        var res = await _authService.RefreshAsync(AccessToken, RefreshToken);
        return Ok(res);
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
    /// <summary>
    /// Reset the password after sending the verification code to the user's email.
    /// </summary>
    [HttpPost("forget-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordFEIDRequest request)
    {
        await _authService.ForgetPasswordAsync(request);
        return Ok("Password reset successful.");
    }

    // Get Verification Code To Reset Password
    /// <summary>
    /// Gets the verification code to reset the password.
    /// </summary>
    [HttpPost("send-reset-code")]
    [AllowAnonymous]
    public async Task<IActionResult> SendResetCode([FromBody] GetVerificationCodeRequest request)
    {
        await _authService.SendResetCodeAsync(request);
        return Ok("Verification code sent successfully.");
    }

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
    /// Logs out the current user and blacklists the access token.
    /// </summary>
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync(AccessToken);
        return Ok("Logout successful.");
    }
}