using Microsoft.Extensions.DependencyInjection;
using TheBoard.Application.Services;

namespace TheBoard.Application.Misc;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProjectsService, ProjectsService>();
        services.AddScoped<IBoardItemsService, BoardItemsService>();

        return services;
    }
}