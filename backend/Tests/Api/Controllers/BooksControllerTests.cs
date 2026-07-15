using Api.Controllers;
using Api.Services;
using Core.DTOs;
using Core.Infrastructure;
using Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Tests.Api.Controllers;

public class BooksControllerTests
{
    [Test]
    public void StartSearch_ReturnsOk()
    {
        var store = new SearchRequestStore();
        var resolver = Substitute.For<AiBookResolver>(Substitute.For<ILlmService>(), Substitute.For<ILogger<AiBookResolver>>());
        var controller = new Books();
        SetupControllerContext(controller);

        var result = controller.StartSearch(store, resolver, "test query");

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public void StartSearch_ReturnsStartSearchResult()
    {
        var store = new SearchRequestStore();
        var resolver = Substitute.For<AiBookResolver>(Substitute.For<ILlmService>(), Substitute.For<ILogger<AiBookResolver>>());
        var controller = new Books();
        SetupControllerContext(controller);

        var result = controller.StartSearch(store, resolver, "test query");

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult!.Value, Is.InstanceOf<StartSearchResult>());
    }

    [Test]
    public void StartSearch_CreatesEntryInStore()
    {
        var store = new SearchRequestStore();
        var resolver = Substitute.For<AiBookResolver>(Substitute.For<ILlmService>(), Substitute.For<ILogger<AiBookResolver>>());
        var controller = new Books();
        SetupControllerContext(controller);

        var result = controller.StartSearch(store, resolver, "test");

        var okResult = result.Result as OkObjectResult;
        var searchResult = okResult!.Value as StartSearchResult;
        var channel = store.Get(searchResult!.RequestId);
        Assert.That(channel, Is.Not.Null);
    }

    [Test]
    public async Task StartSearch_BackgroundTask_CompletesChannel()
    {
        var store = new SearchRequestStore();
        var llm = Substitute.For<ILlmService>();
        llm.OneShotAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("""{"matches":[],"success":true}""");
        var resolver = new AiBookResolver(llm, Substitute.For<ILogger<AiBookResolver>>());
        var controller = new Books();
        SetupControllerContext(controller);

        var actionResult = controller.StartSearch(store, resolver, "test");
        var okResult = actionResult.Result as OkObjectResult;
        var searchResult = okResult!.Value as StartSearchResult;
        var channel = store.Get(searchResult!.RequestId)!;

        // wait for the background task to write
        await channel.Reader.WaitToReadAsync();
        var completed = channel.Reader.TryRead(out var data);

        Assert.Multiple(() =>
        {
            Assert.That(completed, Is.True);
            Assert.That(data, Is.Not.Null);
        });
    }

    [Test]
    public async Task Stream_UnknownId_Sets404()
    {
        var store = new SearchRequestStore();
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        var controller = new Books() { ControllerContext = new ControllerContext { HttpContext = httpContext } };

        await controller.Stream(Guid.NewGuid(), store, CancellationToken.None);

        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task Stream_KnownId_WritesData()
    {
        var store = new SearchRequestStore();
        var (id, channel) = store.Create();
        await channel.Writer.WriteAsync(new BookCandidateResponse { Success = true });
        channel.Writer.Complete();

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        var controller = new Books() { ControllerContext = new ControllerContext { HttpContext = httpContext } };

        await controller.Stream(id, store, CancellationToken.None);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        Assert.That(body, Does.StartWith("data: "));
    }

    [Test]
    public async Task Stream_WritesSseData()
    {
        var store = new SearchRequestStore();
        var (id, channel) = store.Create();
        var candidate = new BookCandidateResponse
        {
            Success = true,
            Matches = [new BookCandidate { Title = "The Hobbit" }]
        };
        await channel.Writer.WriteAsync(candidate);
        channel.Writer.Complete();

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        var controller = new Books() { ControllerContext = new ControllerContext { HttpContext = httpContext } };

        await controller.Stream(id, store, CancellationToken.None);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var contentType = httpContext.Response.ContentType;

        Assert.Multiple(() =>
        {
            Assert.That(contentType, Is.EqualTo("text/event-stream"));
            Assert.That(body, Does.StartWith("data: "));
            Assert.That(body, Does.Contain("The Hobbit"));
        });
    }

    private static void SetupControllerContext(ControllerBase controller)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.RequestServices = new ServiceCollection().BuildServiceProvider();
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }
}
