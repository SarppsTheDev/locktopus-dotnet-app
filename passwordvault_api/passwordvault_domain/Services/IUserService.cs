using passwordvault_domain.Entities;

namespace passwordvault_domain.Services;

public interface IUserService
{
    Task<User> UpdateUserPersonalInfo(string userId, string firstName, string lastName);
}