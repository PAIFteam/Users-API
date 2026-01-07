namespace Users.API.Users.UpdateUser; 

public record UpdateUserCommand(Guid IdUser, string Name, string Login, string Password,
    string Email, DateTime DateBirth, EnumProfile IdProfile) : ICommand<UpdateUserResult>;

public record UpdateUserResult(bool IsSuccess);

internal class UpdateUserCommandHandler(IDocumentSession session, ILogger<UpdateUserCommandHandler> logger)
    : ICommandHandler<UpdateUserCommand, UpdateUserResult>
{
    public async Task<UpdateUserResult> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("UpdateUserCommandHandler.Handle called with {@Command}", command);

        var users = await session.LoadAsync<UserModel>(command.IdUser, cancellationToken);

        if(users is null)
        {
            throw new UserNotFoundException(); 
        }

        users.Name = command.Name;
        users.Login = command.Login;
        users.Password = command.Password;
        users.Email = command.Email;
        users.DateBirth = command.DateBirth;
        users.IdProfile = command.IdProfile;

        session.Update(users);
        await session.SaveChangesAsync(cancellationToken);

        return new UpdateUserResult(true); 
    }
}
