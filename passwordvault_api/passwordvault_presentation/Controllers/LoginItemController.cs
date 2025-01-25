using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using passwordvault_domain.Entities;
using passwordvault_domain.Exceptions;
using passwordvault_domain.Helpers;
using passwordvault_domain.Services;
using passwordvault_presentation.Requests;

namespace passwordvault_presentation.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class LoginItemController(ILogger<LoginItemController> logger, ILoginItemService loginItemService, IUserContextHelper userContext) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody]LoginItemRequest request)
    {
        try
        {
            if(!userContext.IsAuthenticated)
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
                UserId = userContext.UserId
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

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] LoginItemRequest request)
    {
        try
        {
            if (!userContext.IsAuthenticated)
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
                UserId = userContext.UserId
            };

            var updatedLoginItem = await loginItemService.UpdateLoginItem(item);

            return Ok(updatedLoginItem);
        }
        catch (LoginItemNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized("User not authorized to update login item.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating login item");

            return BadRequest("Failed to update login item");
        }
        
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if(!userContext.IsAuthenticated)
            {
                return Unauthorized("User is not logged in.");
            }

            await loginItemService.DeleteLoginItem(id);

            return Ok("Login item deleted");

        }
        catch (LoginItemNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized("User not authorized to update login item.");
        }
        catch (Exception ex)
        {
            return BadRequest("Failed to delete login item");
        }
    }
}