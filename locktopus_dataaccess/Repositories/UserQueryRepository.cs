using Microsoft.EntityFrameworkCore;
using locktopus_domain.Entities;
using locktopus_domain.Repositories;

namespace locktopus_dataaccess.Repositories;

public class UserQueryRepository(AppDbContext dbContext) : IUserQueryRepository
{
    private IQueryable<User> Users => dbContext.Set<User>();
    
    public Task<User> GetById(long id)
    {
        throw new NotSupportedException("User IDs are strings. This method is not supported.");
    }

    public async Task<User> GetById(string id)
    {
        var user = await Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new Exception($"No user exists with ID {id}");
        }

        return user;
    }
}