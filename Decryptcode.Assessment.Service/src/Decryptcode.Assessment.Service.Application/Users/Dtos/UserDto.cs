namespace Decryptcode.Assessment.Service.Application.Users.Dtos;

public sealed record UserDto
{
    public required string Id { get; init; }

    public required string OrdId { get; init; }

    public required string Email { get; init; }

    public required string Name { get; init; }

    public required string Role { get; init; }

    public bool Active { get; init; }

    public required string Bio { get; init; }
}
