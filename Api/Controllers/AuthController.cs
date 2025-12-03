using Api.Extensions;
using Api.Models.Auth;
using Application.DTOs.Auth;
using Application.Enums;
using Application.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<LoginUserResponse>> RegisterUser(
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
    public async Task<ActionResult<LoginUserResponse>> LoginUser([FromBody] LoginUserRequest request)
    {
        LoginUserResponse result = await authService.LoginUser(request);

        return Ok(result);
    }

    [HttpPost]
    [Route("verify-email")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        int userId = User.GetUserId();

        await verificationService.VerifyCode(userId, request.Code);

        return NoContent();
    }

    [HttpPost]
    [Route("send-verification-email")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> SendVerificationEmail()
    {
        int userId = User.GetUserId();

        await verificationService.ResendRegisterCode(userId);

        return NoContent();
    }

    [HttpPost("android/google")]
    public async Task<ActionResult<LoginUserResponse>> GoogleAndroidLogin([FromBody] GoogleLoginRequest request)
    {
        LoginUserResponse result = await authService.LoginWithGoogle(request, GoogleAuthPlatform.Android);

        return Ok(result);
    }

    [HttpPost("web/google")]
    public async Task<ActionResult<LoginUserResponse>> GoogleWebLogin([FromBody] GoogleLoginRequest request)
    {
        LoginUserResponse result = await authService.LoginWithGoogle(request, GoogleAuthPlatform.Web);

        return Ok(result);
    }

    [HttpGet("email-exists")]
    public async Task<ActionResult<bool>> IsEmailExists([FromQuery] string email)
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

        int userId = User.GetUserId();

        await authService.ChangePassword(userId, request);

        return Ok();
    }

    [HttpPost]
    [Route("reset-password")]
    public async Task<IActionResult> PasswordReset(
        [FromBody] ForgotPasswordRequest request,
        [FromServices] IValidator<ForgotPasswordRequest> validator
    )
    {
        ValidationResult? validatorResult = await validator.ValidateAsync(request);

        if (!validatorResult.IsValid)
        {
            return BadRequest(validatorResult.Errors.Select(x => x.ErrorMessage));
        }

        ResetPasswordResult result = await authService.RequestPasswordReset(request);

        string resetLink = $"{request.ClientUrl}/reset-password?email={request.Email}&token={result.Token}";

        await emailService.SendEmailAsync(
            request.Email,
            "Підтвердження зміни пароля",
            $"Для підтвердження зміни пароля перейдіть по посиланню: {resetLink}");

        return Ok();
    }

    [HttpGet("confirm-password-change")]
    public async Task<IActionResult> ConfirmPasswordChange([FromBody] ResetPasswordRequest request)
    {
        await authService.ConfirmPasswordReset(request.Email, request.Token, request.NewPassword);

        return NoContent();
    }
}