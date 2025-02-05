using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using passwordvault_domain.Entities;
using passwordvault_domain.Exceptions;
using passwordvault_domain.Helpers;
using passwordvault_domain.Services;
using passwordvault_presentation.Requests;
using passwordvault_presentation.Responses;

namespace passwordvault_presentation.Controllers;

//TODO: Remove if statement for authentication if not required
[Authorize]
[ApiController]
[Route("[controller]")]
public class LoginItemController(
    ILogger<LoginItemController> logger,
    ILoginItemService loginItemService,
    IUserContextHelper userContext) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] LoginItemRequest request)
    {
        if (!userContext.IsAuthenticated)
        {
            return Unauthorized("User is not logged in.");
        }

        try
        {
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

    [HttpPost("update/{id:int}")]
    public async Task<IActionResult> Update([FromBody] LoginItemRequest request, int id)
    {
        if (!userContext.IsAuthenticated)
        {
            return Unauthorized("User is not logged in.");
        }

        try
        {
            var item = new LoginItem
            {
                LoginItemId = id,
                Title = request.Title,
                Username = request.Username,
                Password = request.Password,
                WebsiteUrl = request.WebsiteUrl,
                Notes = request.Notes,
                UserId = userContext.UserId
            };

            var updatedLoginItem = await loginItemService.UpdateLoginItem(item);

            var response = new LoginItemResponse(
                updatedLoginItem.LoginItemId,
                updatedLoginItem.Title,
                updatedLoginItem.WebsiteUrl,
                updatedLoginItem.Username,
                updatedLoginItem.Password,
                updatedLoginItem.Notes);

            return Ok(response);
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!userContext.IsAuthenticated)
        {
            return Unauthorized("User is not logged in.");
        }

        try
        {
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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        if (!userContext.IsAuthenticated)
        {
            return Unauthorized("User is not logged in.");
        }

        try
        {
            var loginItem = await loginItemService.GetLoginItem(id);

            var response = new LoginItemResponse(
                loginItem.LoginItemId,
                loginItem.Title,
                loginItem.WebsiteUrl,
                loginItem.Username,
                loginItem.Password,
                loginItem.Notes);

            return Ok(response);
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
            return BadRequest("Failed to retrieve login item");
        }
    }

    [HttpGet("list-by-userid")]
    public async Task<IActionResult> GetListByUserId()
    {
        if (!userContext.IsAuthenticated)
        {
            return Unauthorized("User is not logged in.");
        }

        try
        {
            var loginItems = await loginItemService.GetLoginItemsByUserId(userContext.UserId);

            var response = loginItems.Select(loginItem => new LoginItemResponse(loginItem.LoginItemId, loginItem.Title,
                loginItem.WebsiteUrl,
                loginItem.Username, loginItem.Password, loginItem.Notes)).ToList();

            return Ok(response);
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
            return BadRequest("Failed to retrieve login item");
        }
    }

    [HttpGet("generate-password")]
    public async Task<IActionResult> GeneratePassword(int passwordLength, bool useLetters, bool useMixedCase, bool useNumbers, bool useSpecialCharacters)
    {
        if (!userContext.IsAuthenticated)
        {
            return Unauthorized("User is not logged in.");
        }

        try
        {
            var generatedPassword = loginItemService.GenerateRandomPassword(passwordLength, useLetters, useMixedCase, useNumbers, useSpecialCharacters);
            
            return Ok(generatedPassword);
        }
        catch (Exception ex)
        {
            return BadRequest("Failed to generate password");
        }
    }
}