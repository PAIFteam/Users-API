
namespace Users.API.Users.UpdateUser; 

public record UpdateUserRequest(Guid IdUser, string Name, string Login, string Password,
    string Email, DateTime DateBirth, EnumProfile IdProfile);

public record UpdateUserResponse(bool IsSuccess);
public class UpdateUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/users", async (ISender sender, UpdateUserRequest request, CancellationToken cancellationToken) =>
        {
            var command = request.Adapt<UpdateUserCommand>();
            var result = await sender.Send(command, cancellationToken);
            var response = result.Adapt<UpdateUserResponse>();

            return Results.Ok(response);
        })
        .WithName("UpdateUser")
        .Produces<UpdateUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Update User")
        .WithDescription("Update User");
    }
}
