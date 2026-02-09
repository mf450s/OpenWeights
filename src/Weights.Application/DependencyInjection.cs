using Microsoft.Extensions.DependencyInjection;
using Weights.Application.Interfaces;
using Weights.Application.Services;

namespace Weights.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IExerciseService, ExerciseService>();
        services.AddScoped<ITemplateService, TemplateService>();
        services.AddScoped<ISessionService, SessionService>();

        return services;
    }
}
