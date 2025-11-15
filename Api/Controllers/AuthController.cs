using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Application.DTOs.Auth;
using Application.Enums;
using Application.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResetPasswordRequest = Application.DTOs.Auth.ResetPasswordRequest;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IEmailSender emailService,
    IAuthService authService,
    IVerificationService verificationService) : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUser(
        [FromBody] RegisterUserRequest request,
        [FromServices] IValidator<RegisterUserRequest> validator
    )
    {
        ValidationResult? validation = await validator.ValidateAsync(request);

        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        }

        LoginUserResponse result = await authService.RegisterUser(request);

        return Ok(result);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request)
    {
        LoginUserResponse result = await authService.LoginUser(request);

        return Ok(result);
    }

    [HttpPost]
    [Route("verify-email")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> VerifyEmail([FromBody] [Required] string code)
    {
        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return BadRequest("User ID not found or invalid.");
        }

        await verificationService.VerifyCode(userId, code);

        return Ok();
    }

    [HttpPost]
    [Route("send-verification-email")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> SendVerificationEmail()
    {
        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return BadRequest("User ID not found or invalid.");
        }

        await verificationService.ResendRegisterCode(userId);

        return Ok();
    }

    [HttpPost("android/google")]
    public async Task<IActionResult> GoogleAndroidLogin([FromBody] GoogleLoginRequest request)
    {
        LoginUserResponse result = await authService.LoginWithGoogle(request, GoogleAuthPlatform.Android);

        return Ok(result);
    }

    [HttpPost("web/google")]
    public async Task<IActionResult> GoogleWebLogin([FromBody] GoogleLoginRequest request)
    {
        LoginUserResponse result = await authService.LoginWithGoogle(request, GoogleAuthPlatform.Web);

        return Ok(result);
    }

    [HttpGet("email-exists")]
    public async Task<IActionResult> IsEmailExists([FromQuery] string email)
    {
        bool result = await authService.IsEmailExists(email);
        return Ok(result);
    }

    [HttpPost]
    [Route("change-password")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        [FromServices] IValidator<ChangePasswordRequest> validator
    )
    {
        ValidationResult? validation = await validator.ValidateAsync(request);

        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("User ID not found or invalid.");

        await authService.ChangePassword(userId, request);

        return Ok();
    }

    [HttpPost]
    [Route("reset-password")]
    public async Task<IActionResult> PasswordReset(
        [FromBody] ResetPasswordRequest request,
        [FromServices] IValidator<ResetPasswordRequest> validator
    )
    {
        ValidationResult? validatorResult = await validator.ValidateAsync(request);

        if (!validatorResult.IsValid)
        {
            return BadRequest(validatorResult.Errors.Select(x => x.ErrorMessage));
        }

        ResetPasswordResult result = await authService.RequestPasswordReset(request);

        var callbackUrl = Url.Action(
            nameof(ConfirmPasswordChange), "Auth",
            new { userId = result.UserId, token = result.Token },
            protocol: Request.Scheme
        );

        await emailService.SendEmailAsync(
            request.Email,
            "Підтвердження зміни пароля",
            $"Для підтвердження зміни пароля перейдіть по посиланню: {callbackUrl}");

        return Ok();
    }

    [HttpGet("confirm-password-change")]
    public async Task<IActionResult> ConfirmPasswordChange([FromQuery] int userId, [FromQuery] string token)
    {
        await authService.ConfirmPasswordReset(userId, token);

        return Ok();
    }
}