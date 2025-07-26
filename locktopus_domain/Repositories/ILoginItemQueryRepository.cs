using locktopus_domain.Entities;

namespace locktopus_domain.Repositories;

public interface ILoginItemQueryRepository : IQueryRepository<LoginItem>
{
    Task<(List<LoginItem> LoginItems, int TotalCount)> GetLoginItemsByUserId(string userId, string? searchTerm,
        int offset,
        int pageSize = 12);
}