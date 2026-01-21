using Users.Core.Application.UseCases.Users.GetUsers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Users.Core.Domain.Interfaces;
using Users.Infra.Data.Repositories.Users;
using Users.Core.Application.UseCases.Users.PutUser;
using Users.Core.Application.UseCases.Users.GetUserById;

namespace Users.Infra.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfraestructure(this IServiceCollection services)
        {
            //Registro do MediaR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    Assembly.GetExecutingAssembly(),
                    Assembly.GetAssembly(typeof(GetUsersUseCase))!
                    );
                cfg.RegisterServicesFromAssemblies(
                    Assembly.GetExecutingAssembly(),
                    Assembly.GetAssembly(typeof(PutUserUseCase))!
                    );
            });

            //Registro dos Repositorios
            services.AddScoped<IGetUsersRepository, GetUsersRepository>();
            services.AddScoped<IPutUserRepository, PutUserRepository>();

            //Registro dos UseCases
            services.AddScoped<GetUsersUseCase>();
            services.AddScoped<PutUserUseCase>();
            services.AddScoped<GetUserByIdUseCase>();

            return services;
        }
    }
}
