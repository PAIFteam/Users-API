using PAIFGAMES.FCG.Domain.Extensions;
using PAIFGAMES.FCG.Domain.Users.Filter;

namespace Users.API.Models;

public sealed class GetAllUsersQueryParams
{
    public string? Email { get; init; }
    public string? Name { get; init; }
    public string? Role { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
