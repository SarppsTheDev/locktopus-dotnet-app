using passwordvault_domain.Entities;

namespace passwordvault_domain.Services;

public interface ILoginItemService
{
    Task<bool> CreateLoginItem(LoginItem loginItem);
}