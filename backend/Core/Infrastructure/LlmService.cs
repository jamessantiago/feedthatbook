using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure;

/// <summary>
/// Generic LLM service that takes an IChatClient such as gemini, openai, etc...
/// </summary>
public interface ILlmService
{
    /// <summary>
    /// Send one or more chat message to the chat client.  Throws on null messages
    /// </summary>
    Task<ChatResponse> SendMessageAsync(List<ChatMessage> messages, CancellationToken token = default);

    /// <summary>
    /// Sends a simple one-shot system prompt to the chat client and returns the response
    /// </summary>
    Task<string> OneShotAsync(string prompt, string input, CancellationToken token = default);
}

public class LlmService(IChatClientFactory chatClientFactory, ChatOptions chatOptions, ILogger<LlmService> logger)
    : ILlmService
{
    private readonly IChatClient _chatClient = chatClientFactory.CreateClient();

    public Task<ChatResponse> SendMessageAsync(List<ChatMessage> messages, CancellationToken token = default)
    {
        return _chatClient.GetResponseAsync(messages, chatOptions, token);
    }

    public async Task<string> OneShotAsync(string prompt, string input, CancellationToken token = default)
    {
        logger.LogInformation("LlmService::OneShotAsync {input}", input);
        List<ChatMessage> messages = [new(ChatRole.System, prompt), new(ChatRole.User, input)];
        var response = await SendMessageAsync(messages, token);
        logger.LogInformation("LlmService::OneShotAsync ({tokenCount} tok) {responseText}", response.Usage?.TotalTokenCount, response.Text);
        return response.Text;
    }
}