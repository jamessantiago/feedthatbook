using System.Text.Json;
using Core.DTOs;
using Core.Infrastructure;
using Core.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Tests.Core.Services;

public class AiBookResolverTests
{
    private ILlmService _llm = null!;
    private AiBookResolver _resolver = null!;

    [SetUp]
    public void Setup()
    {
        _llm = Substitute.For<ILlmService>();
        _resolver = new AiBookResolver(_llm, Substitute.For<ILogger<AiBookResolver>>());
    }

    [Test]
    public async Task ResolveBook_ValidJson_ReturnsDeserializedResult()
    {
        var response = new BookCandidateResponse
        {
            Success = true,
            Matches = [new BookCandidate { Title = "The Hobbit", Author = "J.R.R. Tolkien" }]
        };
        _llm.OneShotAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(JsonSerializer.Serialize(response));

        var result = await _resolver.ResolveBook("tolkien hobbit");

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.Matches, Has.Count.EqualTo(1));
            Assert.That(result.Matches![0].Title, Is.EqualTo("The Hobbit"));
        });
    }

    [Test]
    public async Task ResolveBook_EmptyResponse_ReturnsFailure()
    {
        _llm.OneShotAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("");

        var result = await _resolver.ResolveBook("unknown");

        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task ResolveBook_InvalidJson_ReturnsFailure()
    {
        _llm.OneShotAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("not json");

        var result = await _resolver.ResolveBook("garbage");

        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task ResolveBook_NullResponse_ReturnsFailure()
    {
        _llm.OneShotAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((string?)null!);

        var result = await _resolver.ResolveBook("test");

        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task ResolveBook_PassesPromptAndQuery()
    {
        _llm.OneShotAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(JsonSerializer.Serialize(new BookCandidateResponse()));

        await _resolver.ResolveBook("mark twain");

        await _llm.Received(1).OneShotAsync(
            Arg.Is<string>(s => s.Contains("book discovery")),
            "mark twain",
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task ResolveBook_WithSnakeCaseJson_DeserializesCorrectly()
    {
        var json = """{"matches":[{"title":"Test","author":"Author","first_publish_year":2000,"explanation":"Match"}],"success":true}""";
        _llm.OneShotAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(json);

        var result = await _resolver.ResolveBook("test");

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.Matches, Has.Count.EqualTo(1));
            Assert.That(result.Matches![0].FirstPublishedYear, Is.EqualTo(2000));
        });
    }

    [Test]
    public async Task ResolveBook_WithJsonPropertyName_DeserializesYear()
    {
        var json = """{"matches":[{"title":"Test","first_publish_year":2000}],"success":true}""";
        _llm.OneShotAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(json);

        var result = await _resolver.ResolveBook("test");

        Assert.That(result.Matches![0].FirstPublishedYear, Is.EqualTo(2000));
    }

    [Test]
    public async Task ResolveBook_NullJson_ReturnsFailure()
    {
        _llm.OneShotAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("null");

        var result = await _resolver.ResolveBook("test");

        Assert.That(result.Success, Is.False);
    }
}
