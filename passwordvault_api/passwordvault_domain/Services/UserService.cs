using Microsoft.Extensions.Logging;
using passwordvault_domain.Entities;
using passwordvault_domain.Repositories;

namespace passwordvault_domain.Services;

public class UserService(IUserRepository userRepository, ILogger<UserService> logger) : IUserService
{
    public async Task<User> UpdateUserPersonalInfo(string userId, string firstName, string lastName)
    {
        var user = await userRepository.GetById(userId);
        
        user.FirstName = firstName;
        user.LastName = lastName;
        
        var updatedUser = await userRepository.Update(user);

        if (updatedUser == null)
        {
            logger.LogError("Could not update user: {User}", user.UserName);
            throw new Exception($"Failed to update user: {user.UserName}"); 
        }
            
        return user;
    }

    public async Task DeleteUser(User user)
    {
        try
        {
            await userRepository.Delete(user.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete user: {User}", user.UserName);
            throw;
        }
        
    }
}