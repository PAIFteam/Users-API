
namespace Users.API.Users.GetUser; 

//public record GetUsersRequest(); 
public record GetUsersResponse(IEnumerable<UserModel> Users);
public class GetUsersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetUsersQuery(), cancellationToken);

            var response = result.Adapt<GetUsersResponse>();

            return Results.Ok(response);
        })
        .WithName("GetUsers")
        .Produces<GetUsersResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Users")
        .WithDescription("Get Users");
    }
}
