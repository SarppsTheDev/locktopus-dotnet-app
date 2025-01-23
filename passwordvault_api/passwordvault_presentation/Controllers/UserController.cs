using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using passwordvault_domain.Entities;
using passwordvault_presentation.Requests;

namespace passwordvault_presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(UserManager<User> userManager, ILogger<UserController> logger) : ControllerBase
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
                return BadRequest(result.Errors);

            return Ok("User registered successfully");
        }
        catch (Exception ex)
        {
            return BadRequest("Failed to register user");
        }
    }
}
