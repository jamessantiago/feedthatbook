using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ChatOptions, GenericChatOptions>();
        services.AddScoped<IChatClientFactory, GeminiChatClientFactory>();
        services.AddScoped<ILlmService, LlmService>();
    }
}