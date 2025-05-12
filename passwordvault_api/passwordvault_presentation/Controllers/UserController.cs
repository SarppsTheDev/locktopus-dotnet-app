using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using passwordvault_domain.Entities;
using passwordvault_domain.Helpers;
using passwordvault_domain.Services;
using passwordvault_presentation.Requests;

namespace passwordvault_presentation.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController(UserManager<User> userManager, IUserService userService, IUserContextHelper userContext, ILogger<UserController> logger) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
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
    
    [HttpPost("update-personal-details")]
    public async Task<IActionResult> UpdatePersonalDetails([FromBody] UserUpdatePersonalInfoRequest request)
    {
        try
        {
            if (userContext.UserId != request.UserId) return Forbid("User is not authorized to update this user's personal info");
            
            var result = await userService.UpdateUserPersonalInfo(request.UserId, request.FirstName, request.LastName);
            
            return Ok("User's personal info successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return BadRequest("Failed to update user's personal details");
        }
    }
    
    [HttpPost("update-email")]
    public async Task<IActionResult> UpdateEmailAddress([FromBody] UpdateEmailRequest request)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(request.CurrentEmail);
            if (user == null) return Unauthorized();
            
            var passwordValid = await userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if(!passwordValid) return Unauthorized("Invalid password");

            var existingEmail = await userManager.FindByEmailAsync(request.NewEmail);
            if (existingEmail != null) return BadRequest("Email address is already in use");

            user.Email = request.NewEmail;
            user.UserName = request.NewEmail;
            user.NormalizedEmail = userManager.NormalizeEmail(request.NewEmail);
            user.NormalizedUserName = userManager.NormalizeName(request.NewEmail);
            
            var result = await userManager.UpdateAsync(user);
            
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new
            {
                Email = user.Email,
                Username = user.UserName,
                IsEmailConfirmed = user.EmailConfirmed
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return BadRequest("Failed to update email");
        }
    }
}
