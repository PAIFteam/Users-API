namespace Users.API.Users.CreateUser;

//will be our endpoint for creating a user

public record CreateUserRequest(string Name, string Login, string Email, DateTime DateBirth, EnumProfile IdProfile);

public record CreateUserResponse(Guid IdUser);
public class CreateUserEndpoint : ICarterModule
{
    // a biblioteca carter auxilia na criação de minimal apis
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users", async (CreateUserRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateUserCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreateUserResponse>();

            return Results.Created($"/users/{response.IdUser}", response);
        })
        .WithName("CreateUser")
        .Produces<CreateUserResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Creates a new user")
        .WithDescription("Creates a new user with the provided information.");
    }
}
