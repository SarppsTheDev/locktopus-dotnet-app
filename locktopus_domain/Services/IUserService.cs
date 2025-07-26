using locktopus_domain.Entities;

namespace locktopus_domain.Services;

public interface IUserService
{
    Task<User> UpdateProfile(string userId, string firstName, string lastName);
    Task DeleteUser(User user);
}