using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using passwordvault_domain.Entities;
using passwordvault_domain.Services;
using passwordvault_presentation.Requests;

namespace passwordvault_presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(UserManager<User> userManager, IEmailService emailService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        try
        {
            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);

            return Ok("User registered successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return BadRequest("Failed to register user");
        }
    }
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgottenPassword([FromBody] ForgottenPasswordRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null) return Accepted();
        
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        
        var link = Url.Action("Reset", "Auth", new { email = request.Email, token }, Request.Scheme)!;
        var html = $"<p>Reset your password: <a href=\"{link}\">Reset Password</a></p>";
        
        await emailService.SendEmailAsync(request.Email, "Reset Password", html);
        
        return Accepted();
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> Reset([FromBody] ResetRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return BadRequest(new { error = "Invalid request." });

        var res = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!res.Succeeded)
            return BadRequest(new { errors = res.Errors.Select(e => e.Description) });

        return Ok(new { status = "Password reset successful" });
    }
}