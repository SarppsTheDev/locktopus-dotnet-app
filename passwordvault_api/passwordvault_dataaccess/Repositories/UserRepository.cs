using Microsoft.EntityFrameworkCore;
using passwordvault_domain.Entities;
using passwordvault_domain.Repositories;

namespace passwordvault_dataaccess.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    private IQueryable<User> Users => dbContext.Set<User>();
    
    public Task<int> Create(User user)
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

    public Task<int> Delete(int id)
    {
        throw new NotImplementedException();
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