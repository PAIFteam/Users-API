namespace Users.API.Users.DeleteUser;

//public record DeleteUserRequest(Guid IdUser);
public record DeleteUserResponse(bool IsSuccess);
public class DeleteUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/users/{idUser:guid}", async (ISender sender, Guid idUser, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new DeleteUserCommand(idUser));
            var response = result.Adapt<DeleteUserResponse>();
            return Results.Ok(response);
        })
        .WithName("DeleteUser")
        .Produces<DeleteUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Delete User")
        .WithDescription("Delete User");
    }
}
