using BuildingBlocks.CQRS;

namespace Users.API.Users.CreateUser;

//will be our logical handler for creating a user

public record CreateUserCommand(string Name, string Login, string Password, string Email, DateTime DateBirth, EnumProfile IdProfile)
    : ICommand<CreateUserResult>;
public record CreateUserResult(Guid IdUser);
internal class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserResult>
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


        return new CreateUserResult(Guid.NewGuid());
    }
}
