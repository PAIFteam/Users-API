
namespace Users.API.Users.GetUserById; 

public record GetUserByIdQuery(Guid IdUser) : IQuery<GetUserByIdResult>; 
public record GetUserByIdResult(UserModel Users);
internal class GetUserByIdQueryHandler(IDocumentSession session, ILogger<GetUserByIdQueryHandler> logger)
    : IQueryHandler<GetUserByIdQuery, GetUserByIdResult>
{
    public async Task<GetUserByIdResult> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("GetUserByIdQuery.Handle called with {@Query}", query);

        var users = await session.LoadAsync<UserModel>(query.IdUser, cancellationToken);
        
        if (users is null)
        {
            throw new UserNotFoundException(); 
        }

        return new GetUserByIdResult(users); 
    }
}
