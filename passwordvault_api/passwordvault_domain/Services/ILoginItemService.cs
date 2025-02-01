using passwordvault_domain.Entities;

namespace passwordvault_domain.Services;

public interface ILoginItemService
{
    Task<int> CreateLoginItem(LoginItem loginItem);
    Task<LoginItem> UpdateLoginItem(LoginItem loginItem);
    Task DeleteLoginItem(int loginItemId);
    Task<LoginItem> GetLoginItem(int id);
    Task<List<LoginItem>> GetLoginItemsByUserId(string userId);
}