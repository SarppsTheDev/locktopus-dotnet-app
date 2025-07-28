using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using locktopus_domain.Entities;
using locktopus_domain.Helpers;
using locktopus_domain.Services;
using locktopus_presentation.Requests;
using locktopus_presentation.Responses;

namespace locktopus_presentation.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController(UserManager<User> userManager, IUserService userService, IUserContextHelper userContext, ILogger<UserController> logger) : ControllerBase
{
    [HttpPost("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            var result = await userService.UpdateProfile(userContext.UserId, request.FirstName, request.LastName);
            
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

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var user = await userManager.FindByIdAsync(userContext.UserId);

            var response = new UserProfileResponse
            (
                user.Email,
                user.FirstName,
                user.LastName,
                user.UserName
            );
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest("Failed to retrieve user's profile");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser()
    {
        try
        {
            var user = await userManager.FindByIdAsync(userContext.UserId);
            
            await userService.DeleteUser(user);

            return Ok("User account has been deleted");
        }
        catch (Exception ex)
        {
            return BadRequest("Failed to delete user's account");
        }
    }
}
