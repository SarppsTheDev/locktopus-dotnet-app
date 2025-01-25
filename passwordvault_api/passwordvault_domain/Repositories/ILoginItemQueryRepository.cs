using passwordvault_domain.Entities;

namespace passwordvault_domain.Repositories;

public interface ILoginItemQueryRepository : IQueryRepository<LoginItem>
{
    Task<List<LoginItem>> GetLoginItemsByUserId(string userId);
}