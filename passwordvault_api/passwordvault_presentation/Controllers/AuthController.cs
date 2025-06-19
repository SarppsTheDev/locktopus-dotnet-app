using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using passwordvault_domain.Entities;
using passwordvault_presentation.Requests;

namespace passwordvault_presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(UserManager<User> userManager, ILogger<AuthController> logger) : ControllerBase
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
}