using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using passwordvault_domain.Entities;
using passwordvault_domain.Repositories;
using passwordvault_domain.Services;

namespace passwordvault_tests;

public class Tests
{
    private ILoginItemService _loginItemService;
    private ILoginItemRepository _loginItemRepository;
    private User _testUser;
    
    [SetUp]
    public void Setup()
    {
        var fakeLogger = new Mock<ILogger<LoginItemService>>();
        var fakeConfiguration = new Mock<IConfiguration>();
        
        // TODO: Refactor this by including in a base test class or mocking IConfig differently (if at all)
        fakeConfiguration.Setup(c => c["EncryptionSecrets:Key"]).Returns("5cbw0sI/b8mrG8XE+6n+WvUWqJNuzsTKCSSOfK2oxNI=");
        fakeConfiguration.Setup(c => c["EncryptionSecrets:IV"]).Returns("xWQpmTGjnOd9sf1wbuhGww==");
        
        _loginItemRepository = new Mock<ILoginItemRepository>().Object;
        
        _loginItemService = new LoginItemService(
            _loginItemRepository,
            fakeConfiguration.Object,
            fakeLogger.Object
            );

        _testUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "dee@gmail.com",
            Email = "dee@gmail.com",
            FirstName = "Dee",
            LastName = "Appiagyei"
        };
    }

    [Test]
    public async Task GivenALoginItem_WhenCreateLoginItemIsCalled_ThenLoginItemIsCreated()
    {
        var loginItem = new LoginItem
        {
            Title = "Netflix",
            WebsiteUrl = "https://www.netflix.com",
            Username = "deeappiagyei",
            Password = "testP@ss123!",
            UserId = _testUser.Id
        };
        
        var result = await _loginItemService.CreateLoginItem(loginItem);
        
        Assert.That(result, Is.True);
    }
}