using Azure.Storage.Queues;
using Bogus;
using BusinessLogic.Models;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NSubstitute;

namespace UserFunctions.Tests.Unit;

public class CreateUserTests
{
    private readonly ILogger<CreateUser> _logger = Substitute.For<ILogger<CreateUser>>();
    private UserInfoDBContextFactory userInfoDBContextFactory = Substitute.For<UserInfoDBContextFactory>();
    private readonly QueueServiceClient _queueServiceClient = Substitute.For<QueueServiceClient>();
    private readonly Faker<User> _userGenerator =
        new Faker<User>()
            .RuleFor(x => x.FirstName, faker => faker.Lorem.Random.Words())
            .RuleFor(x => x.LastName, faker => faker.Lorem.Random.Words());
    private readonly IUserRepository _userRepository;

    private readonly CreateUser _sut;

    public CreateUserTests()
    {
        _userRepository = new UserRepository(userInfoDBContextFactory);
        _sut = new CreateUser(_logger, _userRepository, _queueServiceClient);
    }

    [Fact]
    public async Task Post_ShouldReturnOkObjectResultWithCreatedUser_WhenCalledWithValidUserDetails()
    {
        // Arrange
        var createUserRequest = _userGenerator.Generate();

        Mock<HttpRequest> mockRequest = CreateMockRequest(createUserRequest);

        // Act
        IActionResult response = await _sut.Run(mockRequest.Object);

        // Assert
        ObjectResult objectResponse = Assert.IsType<ObjectResult>(response);
        Assert.Equal(((IStatusCodeActionResult)response).StatusCode, 200);
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequestResul_WhenCalledWithEmptyUserDetails()
    {
        // Arrange
        var createUserRequest = new User
        {
            FirstName = null,
            LastName = null
        };

        Mock<HttpRequest> mockRequest = CreateMockRequest(createUserRequest);

        // Act
        IActionResult response = await _sut.Run(mockRequest.Object);

        // Assert
        ObjectResult objectResponse = Assert.IsType<ObjectResult>(response);
        Assert.Equal(((IStatusCodeActionResult)response).StatusCode, 400);
    }

    private static Mock<HttpRequest> CreateMockRequest(object body)
    {
        var ms = new MemoryStream();
        var sw = new StreamWriter(ms);

        var json = JsonConvert.SerializeObject(body);

        sw.Write(json);
        sw.Flush();

        ms.Position = 0;

        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(x => x.Body).Returns(ms);

        return mockRequest;
    }
}
