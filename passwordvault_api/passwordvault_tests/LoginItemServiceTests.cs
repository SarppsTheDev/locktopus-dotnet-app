using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using passwordvault_domain.Entities;
using passwordvault_domain.Repositories;
using passwordvault_domain.Services;
using passwordvault_tests.Mocks.Repositories;

namespace passwordvault_tests;

[TestFixture]
public class LoginItemServiceTests
{
    private Mock<ILoginItemRepository> _mockRepository;
    private Mock<ILogger<LoginItemService>> _mockLogger;
    private Mock<IConfiguration> _mockConfiguration;
    private LoginItemService _service;

    [SetUp]
    public void Setup()
    {
        // Mock IConfiguration
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(config => config["EncryptionSecrets:Key"]).Returns("mockEncryptionKey123456");
        _mockConfiguration.Setup(config => config["EncryptionSecrets:IV"]).Returns("mockEncryptionIV123456");

        // Initialize the mock repository
        _mockRepository = MockLoginItemRepository.GetMock("user123");

        // Mock logger
        _mockLogger = new Mock<ILogger<LoginItemService>>();

        // Initialize the service with mocks
        _service = new LoginItemService(_mockRepository.Object, _mockConfiguration.Object, _mockLogger.Object);
    }

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
        var result = await _service.CreateLoginItem(loginItem);

        // Assert
        Assert.That(result, Is.True);
        _mockRepository.Verify(repo => repo.Create(It.Is<LoginItem>(li =>
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
        _mockRepository
            .Setup(repo => repo.Create(It.IsAny<LoginItem>()))
            .ReturnsAsync(0);

        // Act
        var result = await _service.CreateLoginItem(loginItem);

        // Assert
        Assert.That(result, Is.False);
        _mockLogger.Verify(log => log.LogError(
            It.IsAny<string>(),
            loginItem.Title
        ), Times.Once);
    }
}