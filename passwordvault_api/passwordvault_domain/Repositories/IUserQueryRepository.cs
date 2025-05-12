using passwordvault_domain.Entities;

namespace passwordvault_domain.Repositories;

public interface IUserQueryRepository : IQueryRepository<User>
{
    // Methods are inherited
    
    Task<User> GetById(string id);
}