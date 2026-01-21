using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PAIFGAMES.FCG.Api.Models;
using PAIFGAMES.FCG.Domain.Extensions;
using PAIFGAMES.FCG.Domain.Users.Commands;
using PAIFGAMES.FCG.Domain.Users.Filter;
using PAIFGAMES.FCG.Domain.Users.Queries;
using Users.API.Models;
using Users.Core.Application.DTOs;
using Users.Core.Application.UseCases.Users.GetUsers;
using Users.Core.Application.UseCases.Users.GetUserById;
using Users.Core.Application.UseCases.Users.PutUser;
using Users.Core.Domain.Entities.RabbitMQ;
using Users.Core.Domain.Interfaces;
using Users.Core.Entities.RabbitMq;
using Users.Core.Domain;
using Users.Core.Domain.Entities;

namespace Users.API.Extensions
{
    public static class UserEndpointsExtensions
    {
        public static void MapUserEndpoints(this WebApplication app, IConfiguration configuration)
        {
            var api = app.MapGroup("/api");

            api.MapPost("/user/register", async (
                RegisterUserCommand command,
                PutUserUseCase putUserUseCase,
                IPublisher publisher,
                RabbitMqConfigurationSettings rabbitSettings,
                ILogger<Program> logger) =>
            {
                try
                {
                    var input = new PutUserInput
                    {
                        Name = command.Name,
                        Login = string.IsNullOrWhiteSpace(command.Login) ? command.Email : command.Login,
                        Password = command.Password,
                        Email = command.Email,
                        DateBirth = DateTime.UtcNow,
                        IdProfile = EnumProfile.Usuario
                    };

                    var result = await putUserUseCase.ExecuteAsync(input);

                    if (result is null || !result.Result)
                        return Results.BadRequest(new ResponseApi(result));

                    var message = new WelcomeCustomerMessage(input.Name, input.Login, input.Email);
                    await publisher.Publish(message, rabbitSettings.GetQueueAdress());

                    return Results.Ok(new ResponseApi(result));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro ao registrar usuário.");
                    return Results.BadRequest(new ResponseApi().AddNotification("Register", "Erro ao processar registro."));
                }
            })
                .WithName("RegisterUser")
                .WithSummary("Registrar usuário")
                .WithDescription("Registra um usuário no banco e publica mensagem no RabbitMQ.")
                .Produces<ResponseApi>(StatusCodes.Status200OK);

            api.MapPost("/user/login", async (
                LoginModel model,
                GetUsersUseCase getUsersUseCase,
                ILogger<Program> logger) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(model.Login) || string.IsNullOrWhiteSpace(model.Password))
                        return Results.BadRequest(new ResponseApi().AddNotification("Login", "Login e senha são obrigatórios."));

                    var input = new GetUsersInput
                    {
                        Login = model.Login,
                        Password = model.Password
                    };

                    var users = await getUsersUseCase.ExecuteAsync(input);
                    var user = users?.FirstOrDefault();

                    if (user is null)
                        return Results.Unauthorized();

                    var role = user.IdProfile switch
                    {
                        EnumProfile.Administrador => "Admin",
                        _ => "User"
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(configuration["Security:AdminKey"].ToString());
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
                            new Claim(ClaimTypes.Name, user.Login ?? string.Empty),
                            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                            new Claim(ClaimTypes.Role, role)
                        }),
                        Expires = DateTime.UtcNow.AddHours(3),
                        SigningCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256Signature
                        )
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    return Results.Ok(new ResponseApi(new { Token = tokenString }));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro ao processar login.");
                    return Results.BadRequest(new ResponseApi().AddNotification("Login", "Erro ao processar login."));
                }
            })
                .WithName("UserLogin")
                .WithSummary("Login")
                .WithDescription("Autentica um usuário no banco e retorna um token JWT.")
                .Produces<ResponseApi>(StatusCodes.Status200OK);

            api.MapGet("/user/{idUser:int}", async (int idUser, GetUserByIdUseCase useCase) =>
            {
                var result = await useCase.ExecuteAsync(idUser);
                if (result is null)
                    return Results.NotFound(new ResponseApi().AddNotification("user", "Usuário não encontrado."));

                return Results.Ok(new ResponseApi(new
                {
                    User = result.User,
                    Games = result.Games
                }));
            })
                .WithName("GetUserById")
                .WithSummary("Buscar usuário por Id")
                .WithDescription("Busca um usuário pelo Id")
                .Produces<ResponseApi>(StatusCodes.Status200OK)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            api.MapGet("/users/all", ([AsParameters] GetAllUsersQueryParams query, HttpResponse response) =>
            {
                var userFilter = new UserFilterModel
                {
                    Email = query.Email,
                    Name = query.Name,
                    Role = query.Role
                };

                var page = new PageFilterModel
                {
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize
                };

                var pagination = new
                {
                    TotalCount = 0,
                    PageSize = page.PageSize,
                    CurrentPage = page.PageNumber,
                    TotalPages = 0,
                    HasPrevious = false,
                    HasNext = false
                };

                response.Headers["X-Pagination"] = System.Text.Json.JsonSerializer.Serialize(pagination);
                response.Headers["Access-Control-Expose-Headers"] = "X-Pagination";

                return Results.Ok(new ResponseApi(new { Users = Array.Empty<object>(), Filter = userFilter, Page = page }));
            })
                .WithName("GetAllUsers")
                .WithSummary("Listar usuários (paginado)")
                .WithDescription("Lista usuários com paginação.")
                .Produces<ResponseApi>(StatusCodes.Status200OK)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            api.MapPut("/user/attach-role", (AttachRoleUserCommand command) =>
                Results.Ok(new ResponseApi(new { Attached = true, Command = command })))
                .WithName("AttachRoleUser")
                .WithSummary("Vincular role ao usuário")
                .WithDescription("Vincula uma role ao usuário")
                .Produces<ResponseApi>(StatusCodes.Status200OK)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            api.MapPut("/user/detach-role", (DetachRoleUserCommand command) =>
                Results.Ok(new ResponseApi(new { Detached = true, Command = command })))
                .WithName("DetachRoleUser")
                .WithSummary("Desvincular role do usuário")
                .WithDescription("Desvincula uma role do usuário")
                .Produces<ResponseApi>(StatusCodes.Status200OK)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            api.MapPut("/user/attach-game", (AttachGameUserCommand command) =>
                Results.Ok(new ResponseApi(new { Attached = true, Command = command })))
                .WithName("AttachGameUser")
                .WithSummary("Vincular jogo ao usuário")
                .WithDescription("Vincula um jogo ao usuário")
                .Produces<ResponseApi>(StatusCodes.Status200OK)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            api.MapPut("/user/detach-game", (DetachGameUserCommand command) =>
                Results.Ok(new ResponseApi(new { Detached = true, Command = command })))
                .WithName("DetachGameUser")
                .WithSummary("Desvincular jogo do usuário")
                .WithDescription("Desvincula um jogo do usuário")
                .Produces<ResponseApi>(StatusCodes.Status200OK)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));
        }

    }
}