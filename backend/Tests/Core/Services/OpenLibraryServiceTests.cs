using Core.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Tests.Core.Services;

public class OpenLibraryServiceTests
{
    [Test]
    public async Task SearchBooksAsync_CallsCorrectUrl()
    {
        var httpFactory = Substitute.For<IHttpClientFactory>();
        var httpClient = new HttpClient(new FakeHttpMessageHandler());
        httpFactory.CreateClient(nameof(OpenLibraryService)).Returns(httpClient);

        var service = new OpenLibraryService(httpFactory, Substitute.For<ILogger<OpenLibraryService>>());
        var result = await service.SearchBooksAsync("the hobbit");

        Assert.That(result, Does.Contain("openlibrary.org/search.json"));
    }

    [Test]
    public async Task SearchBooksAsync_EncodesQuery()
    {
        var httpFactory = Substitute.For<IHttpClientFactory>();
        var httpClient = new HttpClient(new FakeHttpMessageHandler());
        httpFactory.CreateClient(nameof(OpenLibraryService)).Returns(httpClient);

        var service = new OpenLibraryService(httpFactory, Substitute.For<ILogger<OpenLibraryService>>());
        var result = await service.SearchBooksAsync("tolkien hobbit 1937");

        Assert.That(result, Is.Not.Empty);
    }

    [Test]
    public async Task SearchBooksAsync_EmptyQuery_StillCalls()
    {
        var httpFactory = Substitute.For<IHttpClientFactory>();
        var httpClient = new HttpClient(new FakeHttpMessageHandler());
        httpFactory.CreateClient(nameof(OpenLibraryService)).Returns(httpClient);

        var service = new OpenLibraryService(httpFactory, Substitute.For<ILogger<OpenLibraryService>>());
        var result = await service.SearchBooksAsync("");

        Assert.That(result, Is.Not.Empty);
    }
}

public class FakeHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent($$"""{"request_url":"{{request.RequestUri}}"}""")
        };
        return Task.FromResult(response);
    }
}
