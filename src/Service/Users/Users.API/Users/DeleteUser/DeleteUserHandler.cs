
namespace Users.API.Users.DeleteUser; 
public record DeleteUserCommand(Guid IdUser) : ICommand<DeleteUserResult>;
public record DeleteUserResult(bool IsSuccess);
internal class DeleteUserCommandHandler(IDocumentSession session, ILogger<DeleteUserCommandHandler> logger)
    : ICommandHandler<DeleteUserCommand, DeleteUserResult>
{
    public async Task<DeleteUserResult> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("DeleteUserCommandHandler.Handle called with {@Command}", command);

        session.Delete(command.IdUser);
        await session.SaveChangesAsync(cancellationToken);

        return new DeleteUserResult(true);
    }
}
