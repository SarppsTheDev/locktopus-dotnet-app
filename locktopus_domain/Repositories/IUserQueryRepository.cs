using locktopus_domain.Entities;

namespace locktopus_domain.Repositories;

public interface IUserQueryRepository : IQueryRepository<User>
{
    // Methods are inherited
    
    Task<User> GetById(string id);
}