
using Microsoft.AspNetCore.Mvc;
using passwordvault_domain.Entities;
using passwordvault_domain.Services;
using passwordvault_presentation.Requests;

namespace passwordvault_presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginItemController(ILogger<LoginItemController> logger, ILoginItemService loginItemService) : ControllerBase
{
    [HttpPost("create-login-item")]
    public async Task<IActionResult> Register([FromBody]LoginItemRequest request) 
    {
        try
        {
            var item = new LoginItem
            {
                Title = request.Title,
                Username = request.Username,
                Password = request.Password,
                Url = request.Url,
                Notes = request.Notes
            };
            
            var created = loginItemService.CreateLoginItem(item);
            
            if(!created)
                throw new ApplicationException("Could not create login item");
            
            return Ok("User registered successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating login item");
            
            return BadRequest("Failed to create login item");
        }
    }
}