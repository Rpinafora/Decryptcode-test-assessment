using Decryptcode.Assessment.Service.Application.Users.Dtos;
using Decryptcode.Assessment.Service.Application.Users.GetAllUsers;
using Decryptcode.Assessment.Service.Application.Users.GetUserById;
using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using Decryptcode.Assessment.Service.Domain.Repositories;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Decryptcode.Assessment.Service.Domain.UnitTests.Application.Users;

/// <summary>
/// Unit tests for GetAllUsersQueryHandler and GetUserByIdQueryHandler
/// </summary>
public class UserQueryHandlerTests
{
    [Fact]
    public async Task GetAllUsers_WithValidQuery_ReturnsAllUsers()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var handler = new GetAllUsersQueryHandler(repositoryMock.Object);
        var query = new GetAllUsersQuery();
        var users = new[]
        {
            CreateTestUser("user-001", "Alice Chen", "admin"),
            CreateTestUser("user-002", "Bob Smith", "member"),
            CreateTestUser("user-003", "Carol Davis", "member")
        };

        var userDtos = new List<UserDto>
        {
            new UserDto { Id = users[0].Id, OrdId = "org-001", Email = users[0].Email, Name = users[0].Name, Role = users[0].Role, Active = users[0].Active, Bio = users[0].Bio },
            new UserDto { Id = users[1].Id, OrdId = "org-001", Email = users[1].Email, Name = users[1].Name, Role = users[1].Role, Active = users[1].Active, Bio = users[1].Bio },
            new UserDto { Id = users[2].Id, OrdId = "org-001", Email = users[2].Email, Name = users[2].Name, Role = users[2].Role, Active = users[2].Active, Bio = users[2].Bio }
        };

        repositoryMock.Setup(r => r.GetAllFiltered(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<bool?>(), 
            It.IsAny<Expression<Func<User, UserDto>>>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(userDtos);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Content);
        Assert.Equal(3, result.Content.Count());
    }

    [Fact]
    public async Task GetAllUsers_WithNoUsers_ReturnsEmptyList()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var handler = new GetAllUsersQueryHandler(repositoryMock.Object);
        var query = new GetAllUsersQuery();
        var emptyDtos = Enumerable.Empty<UserDto>();

        repositoryMock.Setup(r => r.GetAllFiltered(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<bool?>(), 
            It.IsAny<Expression<Func<User, UserDto>>>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyDtos);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Empty(result.Content!);
    }

    [Fact]
    public async Task GetUserById_WithValidId_ReturnsUser()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var handler = new GetUserByIdQueryHandler(repositoryMock.Object);
        var userId = "user-001";
        var query = new GetUserByIdQuery { Id = userId };
        var user = CreateTestUser(userId, "Alice Chen", "admin");
        var userDto = new UserDto { Id = user.Id, OrdId = "org-001", Email = user.Email, Name = user.Name, Role = user.Role, Active = user.Active, Bio = user.Bio };

        repositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<Expression<Func<User, UserDto>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userDto);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Content);
        Assert.Equal("Alice Chen", result.Content.Name);
    }

    [Fact]
    public async Task GetUserById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var handler = new GetUserByIdQueryHandler(repositoryMock.Object);
        var query = new GetUserByIdQuery { Id = "invalid-id" };
        UserDto? nullDto = null;

        repositoryMock.Setup(r => r.GetByIdAsync("invalid-id", It.IsAny<Expression<Func<User, UserDto>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(nullDto);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Content);
    }

    // Helper methods
    private User CreateTestUser(string id, string name, string role) =>
        User.Create(id, "org-001", $"{id}@email.com", name, role, true, $"Bio for {name}");
}
