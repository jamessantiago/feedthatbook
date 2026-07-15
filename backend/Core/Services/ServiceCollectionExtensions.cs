using Microsoft.Extensions.DependencyInjection;

namespace Core.Services;

public static class ServiceCollectionExtensions
{
    public static void AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<OpenLibraryService>();
        services.AddScoped<AiBookResolver>();
    }
}