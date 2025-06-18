using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using passwordvault_domain.Entities;
using passwordvault_domain.Helpers;
using passwordvault_domain.Services;
using passwordvault_presentation.Requests;
using passwordvault_presentation.Responses;

namespace passwordvault_presentation.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController(UserManager<User> userManager, IUserService userService, IUserContextHelper userContext, ILogger<UserController> logger) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
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
            var result = await userService.UpdateUserPersonalInfo(userContext.UserId, request.FirstName, request.LastName);
            
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
