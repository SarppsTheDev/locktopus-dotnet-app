using Microsoft.EntityFrameworkCore;
using passwordvault_domain.Entities;
using passwordvault_domain.Exceptions;
using passwordvault_domain.Repositories;

namespace passwordvault_dataaccess.Repositories;

public class LoginItemQueryRepository(AppDbContext dbContext) : ILoginItemQueryRepository
{
    private IQueryable<LoginItem> LoginItems => dbContext.Set<LoginItem>();
    public async Task<LoginItem> GetById(int id)
    {
        var loginItem = await LoginItems.FirstOrDefaultAsync(li => li.LoginItemId == id);

        if (loginItem == null)
        {
            throw new LoginItemNotFoundException($"No login item exists with ID {id}");
        }

        return loginItem;
    }

    public async Task<List<LoginItem>> GetLoginItemsByUserId(string userId)
    {
        return await LoginItems.Where(li => li.UserId == userId).ToListAsync();
    }
}