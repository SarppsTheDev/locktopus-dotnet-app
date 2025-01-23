using Moq;
using passwordvault_domain.Entities;
using passwordvault_domain.Repositories;

namespace passwordvault_tests.Mocks.Repositories;

internal class MockLoginItemRepository
{
    public static Mock<ILoginItemRepository> GetMock(string userId)
    {
        var mock = new Mock<ILoginItemRepository>();

        var loginItems = new List<LoginItem>()
        {
            new LoginItem
            {
                LoginItemId = 1,
                Title = "Netflix",
                WebsiteUrl = "https://www.netflix.com",
                Username = "deeappiagyei",
                Password = "testP@ss123!",
                UserId = userId
            }
        };

        // Setup for Create
        mock.Setup(repo => repo.Create(It.IsAny<LoginItem>()))
            .ReturnsAsync((LoginItem loginItem) =>
            {
                loginItems.Add(loginItem);
                return loginItems.Count; // Simulate the ID of the new item
            });

        // Setup for Update
        mock.Setup(repo => repo.Update(It.IsAny<LoginItem>()))
            .Callback((LoginItem loginItem) =>
            {
                var existing = loginItems.FirstOrDefault(li => li.LoginItemId == loginItem.LoginItemId);
                if (existing != null)
                {
                    existing.Title = loginItem.Title;
                    existing.Password = loginItem.Password;
                }
            });

        // Setup for Delete
        mock.Setup(repo => repo.Delete(It.IsAny<int>()))
            .Callback((LoginItem loginItem) =>
            {
                loginItems.RemoveAll(li => li.LoginItemId == loginItem.LoginItemId);
            });

        return mock;
    }
}