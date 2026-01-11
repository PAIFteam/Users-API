using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Users.Core.Application.DTOs;
using Users.Core.Application.UseCases.Users.GetUsers;
using Users.Core.Application.UseCases.Users.PutUser;
using Users.Core.Domain;
using Users.Core.Domain.Entities;


namespace Users.API.Extensions
{
   

    public static class UserEndpointsExtensions
    {
        public static void MapUserEndpoints(this WebApplication app)
        {

            app.MapGet("/GetUsers", async (string? login,
            string? password,
            string? email,
            GetUsersUseCase getUsersUseCase,
            ILogger<Program> logger) =>
            {
                try
                {
                    var input = new GetUsersInput
                    {
                        Login = login,
                        Password = password,
                        Email = email
                    };

                    var result = await getUsersUseCase.ExecuteAsync(input);

                    if (result == null || !result.Any())
                        return Results.NotFound("Nenhum usuário encontrado com os critérios fornecidos.");

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Um erro ocorreu ao processar a solicitação GetUsers.");
                    return Results.BadRequest("Um erro ocorreu ao processar sua solicitação.");
                }

            })
               .WithName("GetUsers")
               .WithDescription("Retorna usuários com base nos critérios fornecidos")
               .Produces<IEnumerable<User>>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound)
               .Produces(StatusCodes.Status400BadRequest);
            //.Produces(StatusCodes.Status401Unauthorized)
            //.RequireAuthorization(policy => policy.RequireRole("Admin"));


            app.MapPut("/PutUser", async (
            string name,
            string login,
            string password,
            string email,
            DateTime dateBirth,
            EnumProfile idProfile,
            PutUserUseCase putUserUseCase,
            ILogger<Program> logger) =>
            {

                try
                {
                    var input = new PutUserInput
                    {
                        Name = name,
                        Login = login,
                        Password = password,
                        Email = email,
                        DateBirth = dateBirth,
                        IdProfile = idProfile
                    };

                    var result = await putUserUseCase.ExecuteAsync(input);

                    if (result == null)
                        return Results.NotFound("Nenhum usuário encontrado com os critérios fornecidos.");

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Um erro ocorreu ao processar a solicitação PutUser.");
                    return Results.BadRequest("Um erro ocorreu ao processar sua solicitação.");
                }
            })
                .WithName("PutUser")
                .WithDescription("Atualiza um usuário existente")
                .Produces<PutUserOutPut>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status400BadRequest);
            //    .Produces(StatusCodes.Status401Unauthorized)
            //    .RequireAuthorization(policy => policy.RequireRole("Admin"));

            app.MapPost("/Login", async (Users.Core.Application.DTOs.LoginRequest loginRequest, 
                ILogger<Program> logger) =>
            {
                try
                {
                    if (loginRequest.Login == "admin" && loginRequest.Password == "123")
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes("abc123");
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new[]
                            {
                            new Claim(ClaimTypes.Name, loginRequest.Login),
                            new Claim(ClaimTypes.Role, "Admin")
                        }),
                            Expires = DateTime.UtcNow.AddHours(3),
                            SigningCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(key),
                                SecurityAlgorithms.HmacSha256Signature
                            )
                        };

                        var token = tokenHandler.CreateToken(tokenDescriptor);
                        var tokenString = tokenHandler.WriteToken(token);

                        return Results.Ok(new LoginResponse { Token = tokenString });
                    }

                    return Results.Unauthorized();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Um erro ocorreu ao processar a solicitação de login.");
                    return Results.BadRequest("Um erro ocorreu ao processar sua solicitação.");
                }
            })
            .WithName("Login")
            .WithDescription("Autentica um usuário e retorna um token JWT")
            .Produces<LoginResponse>(StatusCodes.Status200OK);
            //    .Produces(StatusCodes.Status401Unauthorized)
            //    .AllowAnonymous();
        }

    }

    public class LoginResponse
    {
        public string Token { get; set; }
    }
}