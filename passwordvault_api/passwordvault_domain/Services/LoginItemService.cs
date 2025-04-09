using System.Text;
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
        var encryptionKey = configuration["EncryptionSecrets:Key"] ??
                            throw new Exception("Encryption key not configured");
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
        await GetLoginItemIfBelongsToCurrentUser(loginItem.LoginItemId);

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
        await GetLoginItemIfBelongsToCurrentUser(loginItemId);

        var rowsAffected = await _loginItemRepository.Delete(loginItemId);

        if (rowsAffected == 0)
        {
            _logger.LogError("Failed to delete login item with ID: {LoginItemId}", loginItemId);
            throw new Exception($"Failed to delete login item with ID: {loginItemId}");
        }

        _logger.LogInformation("Successfully deleted login item with ID: {LoginItemId}", loginItemId);
    }

    public async Task<LoginItem> GetLoginItem(int loginItemId)
    {
        var loginItem = await GetLoginItemIfBelongsToCurrentUser(loginItemId);

        loginItem.Password = _encryptionHelper.Decrypt(loginItem.EncryptedPassword);

        return loginItem;
    }

    public async Task<(List<LoginItem> LoginItems, int TotalCount)> GetLoginItemsByUserId(string userId, string? searchTerm, int offset, int pageSize = 12)
    {
        var paginatedResult = await _loginItemQueryRepository.GetLoginItemsByUserId(userId, searchTerm, offset, pageSize);

        foreach (var loginItem in paginatedResult.LoginItems)
        {
            loginItem.Password = _encryptionHelper.Decrypt(loginItem.EncryptedPassword);
        }

        return paginatedResult;
    }

    public string GenerateRandomPassword(int passwordLength, bool useLetters = true, bool useMixedCase = true, bool useNumbers = true,
        bool useSymbols = true)
    {
        const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numberChars = "0123456789";
        const string symbols = "!@#$%^&*()_-+=<>?";

        var validChars = new StringBuilder();
        var password = new StringBuilder();

        if (!useLetters && !useMixedCase && !useNumbers && !useSymbols)
            throw new ArgumentException("At least one password condition must be selected");
         
        if (useLetters)
        {
            validChars.Append(lowercaseChars);
            if (useMixedCase)
                validChars.Append(uppercaseChars);
        }

        if (useNumbers)
            validChars.Append(numberChars);
        if (useSymbols)
            validChars.Append(symbols);

        if (validChars.Length == 0)
            throw new ArgumentException("At least one character set must be selected.");

        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            var uintBuffer = new byte[sizeof(uint)];

            while (password.Length < passwordLength)
            {
                rng.GetBytes(uintBuffer);
                var num = BitConverter.ToUInt32(uintBuffer, 0);
                password.Append(validChars[(int)(num % (uint)validChars.Length)]);
            }
        }

        return password.ToString();

    }

    private async Task<LoginItem> GetLoginItemIfBelongsToCurrentUser(int loginItemId)
    {
        var userId = _userContext.UserId;
        var loginItem = await _loginItemQueryRepository.GetById(loginItemId);

        if (loginItem.UserId == userId) return loginItem;

        _logger.LogError("User not authorized to access login item");
        throw new UnauthorizedAccessException();
    }
}