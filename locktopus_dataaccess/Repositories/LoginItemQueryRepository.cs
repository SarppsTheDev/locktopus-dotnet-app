using Microsoft.EntityFrameworkCore;
using locktopus_domain.Entities;
using locktopus_domain.Exceptions;
using locktopus_domain.Repositories;

namespace locktopus_dataaccess.Repositories;

public class LoginItemQueryRepository(AppDbContext dbContext) : ILoginItemQueryRepository
{
    private IQueryable<LoginItem> LoginItems => dbContext.Set<LoginItem>();
    public async Task<LoginItem> GetById(long id)
    {
        var loginItem = await LoginItems.FirstOrDefaultAsync(li => li.LoginItemId == id);

        if (loginItem == null)
        {
            throw new LoginItemNotFoundException($"No login item exists with ID {id}");
        }

        return loginItem;
    }

    public async Task<(List<LoginItem> LoginItems, int TotalCount)> GetLoginItemsByUserId(string userId, string? searchTerm, int offset,
        int pageSize = 12)
    {
        var loginItems = await LoginItems
            .Where(li => li.UserId == userId)
            .ToListAsync();
        
        if(!string.IsNullOrEmpty(searchTerm))
            loginItems = loginItems.Where(li => li.Title.ToLower().Contains(searchTerm.ToLower())).ToList();

        var countOfLoginItems = loginItems.Count;

        var paginatedLoginItems = loginItems
            .OrderBy(li => li.Title)
            .Skip(offset)
            .Take(pageSize)
            .ToList();

        return (paginatedLoginItems, countOfLoginItems);
    }
}