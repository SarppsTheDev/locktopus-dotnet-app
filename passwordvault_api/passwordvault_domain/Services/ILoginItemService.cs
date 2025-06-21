using passwordvault_domain.Entities;

namespace passwordvault_domain.Services;

public interface ILoginItemService
{
    Task<int> CreateLoginItem(LoginItem loginItem);
    Task<LoginItem> UpdateLoginItem(LoginItem loginItem);
    Task DeleteLoginItem(int loginItemId);
    Task<LoginItem> GetLoginItem(int id);
    Task<(List<LoginItem> LoginItems, int TotalCount)> GetLoginItemsByUserId(string userId, string? searchTerm,
        int offset, int pageSize);
    string GenerateRandomPassword(int passwordLength, bool useLetters, bool useMixedCase, bool useNumbers,
        bool useSymbols);
}