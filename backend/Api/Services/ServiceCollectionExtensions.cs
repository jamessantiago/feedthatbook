namespace Api.Services;

public static class ServiceCollectionExtensions
{
    public static void AddApiServices(this IServiceCollection services)
    {
        services.AddSingleton<SearchRequestStore>();
    }
}