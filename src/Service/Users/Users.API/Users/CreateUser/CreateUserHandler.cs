using MediatR;
using Users.API.Enums;

namespace Users.API.Users.CreateUser;

//will be our logical handler for creating a user

public record CreateUserCommand(string Name, string Login, string Password, string Email, DateTime DateBirth, EnumProfile IdProfile)
    : IRequest<CreateUserResult>;
public record CreateUserResult(int IdUser);
internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
{
    public Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
