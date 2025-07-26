using Moq;
using locktopus_domain.Entities;

namespace locktopus_tests;

[TestFixture]
public class CreateLoginItemTests : LoginItemServiceTestBase
{
    [Test]
    public async Task CreateLoginItem_ShouldEncryptPasswordAndReturnTrue_WhenRepositoryCreatesSuccessfully()
    {
        // Arrange
        var loginItem = new LoginItem
        {
            Title = "New Login Item",
            WebsiteUrl = "https://example.com",
            Username = "testuser",
            Password = "plaintextpassword",
            UserId = "user123"
        };

        // Act
        var result = await Service.CreateLoginItem(loginItem);

        // Assert
        Assert.That(result, Is.GreaterThan(0));
        MockRepository.Verify(repo => repo.Create(It.Is<LoginItem>(li =>
            li.Title == loginItem.Title &&
            li.Username == loginItem.Username &&
            li.EncryptedPassword != loginItem.Password
        )), Times.Once);
    }

    [Test]
    public async Task CreateLoginItem_ShouldReturnFalse_WhenRepositoryFailsToCreate()
    {
        // Arrange
        var loginItem = new LoginItem
        {
            Title = "New Login Item",
            WebsiteUrl = "https://example.com",
            Username = "testuser",
            Password = "plaintextpassword",
            UserId = "user123"
        };

        // Force the repository to simulate failure
        MockRepository
            .Setup(repo => repo.Create(It.IsAny<LoginItem>()))
            .ReturnsAsync(0);

        // Act
        var result = Assert.ThrowsAsync<Exception>(async () => await Service.CreateLoginItem(loginItem));
    }
}