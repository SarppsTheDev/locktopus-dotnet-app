using passwordvault_domain.Entities;

namespace passwordvault_domain.Services;

public interface ILoginItemService
{
    bool CreateLoginItem(LoginItem loginItem);
}