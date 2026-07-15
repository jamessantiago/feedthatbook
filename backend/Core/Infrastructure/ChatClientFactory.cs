using Core.Services;
using GeminiDotnet;
using GeminiDotnet.Extensions.AI;
using Microsoft.Extensions.AI;

namespace Core.Infrastructure;

/// <summary>
/// Creates an IChatClient, this is the generic microsoft extension interface that generalizes clients such as gemini/openai
/// </summary>
public interface IChatClientFactory
{
    /// <summary>
    /// Create the chat client 
    /// </summary>
    IChatClient CreateClient();
}

public class GeminiChatClientFactory : IChatClientFactory
{
    public IChatClient CreateClient()
    {
        if (string.IsNullOrWhiteSpace(CoreGlobal.Settings.GeminiApiKey))
        {
            throw new ArgumentNullException(
                $"Specify {nameof(CoreGlobal.Settings.GeminiApiKey)} parameter in appsettings.json or environment variables before starting up");
        }

        if (string.IsNullOrWhiteSpace(CoreGlobal.Settings.GeminiModelId))
        {
            throw new ArgumentNullException(
                $"Specify {nameof(CoreGlobal.Settings.GeminiModelId)} parameter in appsettings.json or environment variables before starting up");
        }

        var geminiClient = new GeminiChatClient(new GeminiClientOptions
        {
            ApiKey = CoreGlobal.Settings.GeminiApiKey,
            ModelId = CoreGlobal.Settings.GeminiModelId
        });

        return new ChatClientBuilder(geminiClient).UseFunctionInvocation().Build();
    }
}
