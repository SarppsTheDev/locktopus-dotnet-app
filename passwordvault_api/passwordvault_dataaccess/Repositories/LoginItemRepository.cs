using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using passwordvault_domain.Entities;
using passwordvault_domain.Repositories;

namespace passwordvault_dataaccess.Repositories;

public class LoginItemRepository(AppDbContext dbContext, ILogger<LoginItemRepository> logger) : ILoginItemRepository
{
    private DbSet<LoginItem> LoginItems => dbContext.Set<LoginItem>();
    
    public async Task<int> Create(LoginItem loginItem)
    {
        LoginItems.Add(loginItem);
        await dbContext.SaveChangesAsync();
        return loginItem.LoginItemId;
    }

    public async Task<LoginItem> Update(LoginItem loginItem)
    {
        try
        {
            var existingItem = await LoginItems.FindAsync(loginItem.LoginItemId);

            if (existingItem == null)
            {
                logger.LogError("Could not find login item with id {ItemId}", loginItem.LoginItemId);
                throw new KeyNotFoundException($"Login Item with ID {loginItem.LoginItemId} was not found");
            }

            existingItem.Title = loginItem.Title;
            existingItem.Username = loginItem.Username;
            existingItem.EncryptedPassword = loginItem.EncryptedPassword;
            existingItem.WebsiteUrl = loginItem.WebsiteUrl;
            existingItem.Notes = loginItem.Notes;

            await dbContext.SaveChangesAsync();

            return existingItem;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating login item with ID {LoginItemId}", loginItem.LoginItemId);
            throw;
        }
    }

    public void Delete(LoginItem entity)
    {
        throw new NotImplementedException();
    }
}