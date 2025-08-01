using Microsoft.EntityFrameworkCore;
using locktopus_domain.Entities;
using locktopus_domain.Repositories;

namespace locktopus_dataaccess.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    private DbSet<User> Users => dbContext.Set<User>();
    
    public Task<long> Create(User user)
    {
        throw new NotImplementedException();
    }

    public async Task<User> Update(User user)
    {
        var existingUser = await GetById(user.Id);
        
        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        
        await dbContext.SaveChangesAsync();
        
        return existingUser;
    }

    public Task<long> Delete(long id)
    {
        throw new NotSupportedException("User IDs are strings. This method is not supported.");
    }

    public async Task<int> Delete(string id)
    {
        var user = await GetById(id);
        Users.Remove(user);
        return await dbContext.SaveChangesAsync();
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