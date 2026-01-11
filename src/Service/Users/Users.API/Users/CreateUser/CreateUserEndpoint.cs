namespace Users.API.Users.CreateUser;

/// <summary>
/// Request para criar um novo usuário
/// </summary>
public record CreateUserRequest(
    /// <summary>Nome completo do usuário</summary>
    string Name, 
    /// <summary>Login único do usuário</summary>
    string Login, 
    /// <summary>Email do usuário</summary>
    string Email, 
    /// <summary>Data de nascimento do usuário</summary>
    DateTime DateBirth, 
    /// <summary>Perfil do usuário (Administrador = 1, Usuario = 2)</summary>
    EnumProfile IdProfile);

/// <summary>
/// Response da criação de usuário
/// </summary>
public record CreateUserResponse(
    /// <summary>ID do usuário criado</summary>
    Guid IdUser);

/// <summary>
/// Endpoint para criação de usuários
/// </summary>
public class CreateUserEndpoint : ICarterModule
{
    /// <summary>
    /// Registra as rotas do endpoint
    /// </summary>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/users", async (CreateUserRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateUserCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CreateUserResponse>();
            return Results.Created($"/users/{response.IdUser}", response);
        })
        .WithName("CreateUser")
        .Produces<CreateUserResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Cria um novo usuário")
        .WithDescription("Cria um novo usuário com as informações fornecidas.");
    }
}
