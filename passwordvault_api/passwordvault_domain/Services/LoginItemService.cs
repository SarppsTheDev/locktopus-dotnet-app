using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using passwordvault_domain.Entities;
using passwordvault_domain.Exceptions;
using passwordvault_domain.Helpers;
using passwordvault_domain.Repositories;

namespace passwordvault_domain.Services;

public class LoginItemService : ILoginItemService
{
    private readonly ILoginItemRepository _loginItemRepository;
    private readonly ILoginItemQueryRepository _loginItemQueryRepository;
    private readonly IUserContextHelper _userContext;
    private readonly string _userId;
    private readonly EncryptionHelper _encryptionHelper;
    private readonly ILogger<LoginItemService> _logger;

    public LoginItemService(ILoginItemRepository loginItemRepository,
        ILoginItemQueryRepository loginItemQueryRepository,
        IUserContextHelper userContext,
        IConfiguration configuration,
        ILogger<LoginItemService> logger)
    {
        _loginItemRepository = loginItemRepository;
        _logger = logger;
        _loginItemQueryRepository = loginItemQueryRepository;
        _userContext = userContext;
        var encryptionKey = configuration["EncryptionSecrets:Key"] ?? throw new Exception("Encryption key not configured");
        var encryptionIv = configuration["EncryptionSecrets:IV"] ?? throw new Exception("Encryption IV not configured");
        _encryptionHelper = new EncryptionHelper(encryptionKey, encryptionIv);
    }

    public async Task<int> CreateLoginItem(LoginItem loginItem)
    {
        loginItem.EncryptedPassword = _encryptionHelper.Encrypt(loginItem.Password);
        var createdId = await _loginItemRepository.Create(loginItem);

        if (createdId == 0)
        {
            _logger.LogError("Could not create login item: {LoginItemTitle}", loginItem.Title);
            throw new Exception($"Failed to create login item: {loginItem.Title}");
        }

        _logger.LogInformation("Created login item: {LoginItemTitle}", loginItem.Title);
        return createdId;
    }

    public async Task<LoginItem> UpdateLoginItem(LoginItem loginItem)
    {
        await CheckIfLoginItemBelongsToCurrentUser(loginItem.LoginItemId);
        
        // Encrypt the updated password if provided
        if (!string.IsNullOrEmpty(loginItem.Password))
        {
            loginItem.EncryptedPassword = _encryptionHelper.Encrypt(loginItem.Password);
        }
        
        var updatedItem = await _loginItemRepository.Update(loginItem);

        if (updatedItem == null)
        {
            _logger.LogError("Could not update login item: {LoginItemTitle}", loginItem.Title);
            throw new Exception($"Failed to update login item: {loginItem.Title}");
        }

        _logger.LogInformation("Successfully updated login item: {LoginItemTitle}", updatedItem.Title);
        return updatedItem;
    }

    public async Task DeleteLoginItem(int loginItemId)
    {
        await CheckIfLoginItemBelongsToCurrentUser(loginItemId);
        
        var rowsAffected = await _loginItemRepository.Delete(loginItemId);

        if (rowsAffected == 0)
        {
            _logger.LogError("Failed to delete login item with ID: {LoginItemId}", loginItemId);
            throw new Exception($"Failed to delete login item with ID: {loginItemId}");
        }

        _logger.LogInformation("Successfully deleted login item with ID: {LoginItemId}", loginItemId);
    }

    private async Task CheckIfLoginItemBelongsToCurrentUser(int loginItemId)
    {
        var userId = _userContext.UserId;
        var loginItem = await _loginItemQueryRepository.GetById(loginItemId);

        if (loginItem.UserId != userId)
        {
            _logger.LogError("User not authorized to update login item");
            throw new UnauthorizedAccessException();
        }
    }
}