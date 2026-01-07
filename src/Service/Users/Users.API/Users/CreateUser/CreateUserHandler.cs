namespace Users.API.Users.CreateUser;

//will be our logical handler for creating a user

public record CreateUserCommand(string Name, string Login, string Password, string Email, DateTime DateBirth, EnumProfile IdProfile)
    : ICommand<CreateUserResult>;
public record CreateUserResult(Guid IdUser);
internal class CreateUserCommandHandler(IDocumentSession session) : ICommandHandler<CreateUserCommand, CreateUserResult>
{
    public async Task<CreateUserResult> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var users = new UserModel
        {
            Name = command.Name,
            Login = command.Login,
            Password = command.Password,
            Email = command.Email,
            DateBirth = command.DateBirth,
            IdProfile = command.IdProfile
        };

        //save to database session
        session.Store(users);
        await session.SaveChangesAsync(cancellationToken);

        return new CreateUserResult(users.IdUser);
    }
}
