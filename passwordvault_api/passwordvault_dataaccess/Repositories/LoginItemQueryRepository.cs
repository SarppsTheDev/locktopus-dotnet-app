using passwordvault_domain.Entities;
using passwordvault_domain.Repositories;

namespace passwordvault_dataaccess.Repositories;

public class LoginItemQueryRepository : ILoginItemQueryRepository
{
    public Task<LoginItem> GetById(int id)
    {
        throw new NotImplementedException();
    }
}