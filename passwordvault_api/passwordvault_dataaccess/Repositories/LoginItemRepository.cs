using Microsoft.EntityFrameworkCore;
using passwordvault_domain.Entities;
using passwordvault_domain.Repositories;

namespace passwordvault_dataaccess.Repositories;

public class LoginItemRepository(AppDbContext dbContext) : ILoginItemRepository
{
    private DbSet<LoginItem> LoginItems => dbContext.Set<LoginItem>();
    
    public async Task<int> Create(LoginItem loginItem)
    {
        LoginItems.Add(loginItem);
        await dbContext.SaveChangesAsync();
        return loginItem.LoginItemId;
    }

    public void Update(LoginItem entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(LoginItem entity)
    {
        throw new NotImplementedException();
    }
}