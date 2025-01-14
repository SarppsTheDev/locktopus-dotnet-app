using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using passwordvault_domain.Entities;
using passwordvault_domain.Services;
using passwordvault_presentation.Requests;

namespace passwordvault_presentation.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class LoginItemController(ILogger<LoginItemController> logger, ILoginItemService loginItemService) : ControllerBase
{
    [HttpPost("create-login-item")]
    public async Task<IActionResult> Create([FromBody]LoginItemRequest request) 
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User is not logged in.");
            }
            
            var item = new LoginItem
            {
                Title = request.Title,
                Username = request.Username,
                Password = request.Password,
                WebsiteUrl = request.WebsiteUrl,
                Notes = request.Notes,
                UserId = userId
            };
            
            var created = await loginItemService.CreateLoginItem(item);
            
            return Created();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating login item");
            
            return BadRequest("Failed to create login item");
        }
    }

    [HttpPost("update-login-item")]
    public async Task<IActionResult> Update([FromBody] LoginItemRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User is not logged in.");
            }
            
            var item = new LoginItem
            {
                LoginItemId = request.LoginItemId,
                Title = request.Title,
                Username = request.Username,
                Password = request.Password,
                WebsiteUrl = request.WebsiteUrl,
                Notes = request.Notes,
                UserId = userId
            };
            
            var updatedLoginItem = await loginItemService.UpdateLoginItem(item);

            return Ok(updatedLoginItem);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error updating login item");
            
            return BadRequest("Failed to update login item");
        }
    }
}