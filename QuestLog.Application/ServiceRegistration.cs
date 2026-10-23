using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using QuestLog.Application.Contracts.Services;
using QuestLog.Application.Features;

namespace QuestLog.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ServiceRegistration).Assembly);


        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<IDeveloperService, DeveloperService>();
        services.AddScoped<IGameService, GameService>();

        return services;
    }
}
