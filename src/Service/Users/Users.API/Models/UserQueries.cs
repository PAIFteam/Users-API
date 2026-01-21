namespace PAIFGAMES.FCG.Domain.Users.Queries;

public sealed record GetUserByUIdQuery(Guid UserUId);

public sealed record GetAllUsersQuery(
    PAIFGAMES.FCG.Domain.Users.Filter.UserFilterModel UserFilter,
    PAIFGAMES.FCG.Domain.Extensions.PageFilterModel PageFilter);
