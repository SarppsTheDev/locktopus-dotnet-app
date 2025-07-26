using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using locktopus_domain.Entities;
using locktopus_domain.Services;
using locktopus_presentation.Requests;

namespace locktopus_presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(UserManager<User> userManager, IEmailService emailService, IConfiguration config, ILogger<AuthController> logger) : ControllerBase
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
        
        //TODO: Encapsulate this logic in service
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        
        // URL encode the token for safe transmission
        var encodedToken = System.Web.HttpUtility.UrlEncode(token);
        
        var resetLink = $"{config["AppUrl"]}/reset-password?email={request.Email}&token={encodedToken}";
        
        string html = System.IO.File.ReadAllText("Assets/ResetPasswordEmail.html");
        html = html.Replace("{{ResetLink}}", resetLink);
        
        await emailService.SendEmailAsync(request.Email, "Reset Password", html);
        
        return Accepted();
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> Reset([FromBody] ResetRequest request)
    {
        //TODO: Encapsulate this logic in service
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return BadRequest(new { error = "Invalid request." });

        var res = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!res.Succeeded)
            return BadRequest(new { errors = res.Errors.Select(e => e.Description) });

        return Ok(new { status = "Password reset successful" });
    }
}