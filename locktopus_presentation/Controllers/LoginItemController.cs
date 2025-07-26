using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using passwordvault_domain.Entities;
using passwordvault_domain.Exceptions;
using passwordvault_domain.Helpers;
using passwordvault_domain.Services;
using passwordvault_presentation.Requests;
using passwordvault_presentation.Responses;

namespace passwordvault_presentation.Controllers;

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
        try
        {
            //TODO: Encapsulate this logic in service and entity
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
            //TODO: Encapsulate this logic in service and entity
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
                updatedLoginItem.Notes,
                updatedLoginItem.CreatedAt,
                updatedLoginItem.LastUpdatedAt);

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
            return Forbid("User not authorized to delete login item.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating login item");
            
            return BadRequest("Failed to delete login item");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var loginItem = await loginItemService.GetLoginItem(id);

            var response = new LoginItemResponse(
                loginItem.LoginItemId,
                loginItem.Title,
                loginItem.WebsiteUrl,
                loginItem.Username,
                loginItem.Password,
                loginItem.Notes,
                loginItem.CreatedAt,
                loginItem.LastUpdatedAt);

            return Ok(response);
        }
        catch (LoginItemNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid("User not authorized to update login item.");
        }
        catch (Exception ex)
        {
            return BadRequest("Failed to retrieve login item");
        }
    }

    [HttpGet("list-by-userid")]
    public async Task<ActionResult<PaginatedList<LoginItemResponse>>> GetListByUserId(string? query, int page = 1, int pageSize = 10)
    {
        try
        { 
            var offset = (page - 1) * pageSize;
            
            var (loginItems, totalCount) = await loginItemService.GetLoginItemsByUserId(userContext.UserId, query, offset, pageSize);

            var response = loginItems.Select(item => new LoginItemResponse(item.LoginItemId, item.Title,
                item.WebsiteUrl,
                item.Username, item.Password, item.Notes, item.CreatedAt, item.LastUpdatedAt)).ToList();
            
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            
            var paginatedResponse = new PaginatedList<LoginItemResponse>(response, page, totalPages, totalCount);

            return Ok(paginatedResponse);
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
    public async Task<IActionResult> GeneratePassword(int passwordLength, bool useLetters, bool useMixedCase, bool useNumbers, bool useSymbols)
    {
        try
        {
            var generatedPassword =
                loginItemService.GenerateRandomPassword(passwordLength, useLetters, useMixedCase, useNumbers,
                    useSymbols);

            return Ok(generatedPassword);
        }
        catch (Exception ex)
        {
            return BadRequest("Failed to generate password");
        }
    }
}