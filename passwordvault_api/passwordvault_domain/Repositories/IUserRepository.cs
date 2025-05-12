using passwordvault_domain.Entities;

namespace passwordvault_domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    // Methods are inherited
    Task<User> GetById(string id);
}