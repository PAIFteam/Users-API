namespace Users.API.Users.GetUserById;

//public record GetUserByIdRequest(Guid IdUser); boa pratica, pois o request ja esta representado na query
public record GetUserByIdResponse(UserModel Users);
public class GetUserByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{IdUser}", async (ISender sender, Guid IdUser, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetUserByIdQuery(IdUser), cancellationToken);
            var response = result.Adapt<GetUserByIdResponse>();
            return Results.Ok(response);
        })
        .WithName("GetUserById")
        .Produces<GetUserByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get User By Id")
        .WithDescription("Get User By Id");
    }
}
