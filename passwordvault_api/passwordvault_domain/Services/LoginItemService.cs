using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using passwordvault_domain.Entities;
using passwordvault_domain.Helpers;
using passwordvault_domain.Repositories;

namespace passwordvault_domain.Services;

public class LoginItemService: ILoginItemService
{
    private readonly ILoginItemRepository _loginItemRepository;
    private readonly EncryptionHelper _encryptionHelper;
    private readonly ILogger<LoginItemService> _logger;
    
    public LoginItemService(ILoginItemRepository loginItemRepository, IConfiguration configuration, ILogger<LoginItemService> logger)
    {
        _loginItemRepository = loginItemRepository;
        _logger = logger;
        var encryptionKey = configuration["EncryptionSecrets:Key"] ?? throw new Exception("Encryption key not configured");
        var encryptionIv = configuration["EncryptionSecrets:IV"] ?? throw new Exception("Encryption IV not configured");
        _encryptionHelper = new EncryptionHelper(encryptionKey, encryptionIv);
    }

    public async Task<bool> CreateLoginItem(LoginItem loginItem)
    {
       loginItem.EncryptedPassword = _encryptionHelper.Encrypt(loginItem.Password);
       var createdId = await _loginItemRepository.Create(loginItem);

       if (createdId == 0)
       {
           _logger.LogError("Could not create login item: {LoginItemTitle}", loginItem.Title);
           return false;
       }
       
       _logger.LogInformation("Created login item: {LoginItemTitle}", loginItem.Title);
       return true;
    }
}