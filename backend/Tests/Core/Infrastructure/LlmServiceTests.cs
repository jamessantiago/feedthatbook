using Core.Infrastructure;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Tests.Core.Infrastructure;

public class LlmServiceTests
{
    private IChatClientFactory _factory = null!;
    private IChatClient _chatClient = null!;
    private ILogger<LlmService> _logger = null!;
    private ChatOptions _options = null!;
    private LlmService _service = null!;

    [SetUp]
    public void Setup()
    {
        _chatClient = Substitute.For<IChatClient>();
        _factory = Substitute.For<IChatClientFactory>();
        _factory.CreateClient().Returns(_chatClient);
        _logger = Substitute.For<ILogger<LlmService>>();
        _options = new ChatOptions();

        _service = new LlmService(_factory, _options, _logger);
    }

    [Test]
    public async Task SendMessageAsync_DelegatesToChatClient()
    {
        var messages = new List<ChatMessage> { new(ChatRole.User, "hello") };
        var expected = new ChatResponse(new ChatMessage(ChatRole.Assistant, "hi"));
        _chatClient.GetResponseAsync(messages, _options, Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await _service.SendMessageAsync(messages);

        Assert.That(result.Text, Is.EqualTo("hi"));
    }

    [Test]
    public async Task OneShotAsync_ReturnsResponseText()
    {
        var response = new ChatResponse(new ChatMessage(ChatRole.Assistant, "response text"));
        _chatClient.GetResponseAsync(
            Arg.Any<IList<ChatMessage>>(),
            Arg.Any<ChatOptions?>(), Arg.Any<CancellationToken>())
            .Returns(response);

        var result = await _service.OneShotAsync("system prompt", "user input");

        Assert.That(result, Is.EqualTo("response text"));
    }

    [Test]
    public async Task OneShotAsync_OnException_Propagates()
    {
        _chatClient.GetResponseAsync(
            Arg.Any<IList<ChatMessage>>(),
            Arg.Any<ChatOptions?>(), Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("API error"));

        Assert.That(() => _service.OneShotAsync("prompt", "input"),
            Throws.InvalidOperationException);

        _logger.Received(1).Log(
            Arg.Is<LogLevel>(l => l == LogLevel.Information),
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [TearDown]
    public void TearDown()
    {
        _chatClient.Dispose();
    }

    [Test]
    public async Task OneShotAsync_BuildsSystemAndUserMessages()
    {
        var response = new ChatResponse(new ChatMessage(ChatRole.Assistant, "ok"));
        _chatClient.GetResponseAsync(
            Arg.Any<IList<ChatMessage>>(),
            Arg.Any<ChatOptions?>(), Arg.Any<CancellationToken>())
            .Returns(response);

        await _service.OneShotAsync("system-prompt", "user-query");

        await _chatClient.Received(1).GetResponseAsync(
            Arg.Is<IList<ChatMessage>>(msgs =>
                msgs.Count == 2 &&
                msgs[0].Role == ChatRole.System &&
                msgs[0].Text == "system-prompt" &&
                msgs[1].Role == ChatRole.User &&
                msgs[1].Text == "user-query"),
            Arg.Any<ChatOptions?>(),
            Arg.Any<CancellationToken>());
    }
}
