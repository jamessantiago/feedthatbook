using Core.Infrastructure;
using Core.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Tests.Core.Infrastructure;

public class GenericChatOptionsTests
{
    [Test]
    public void Constructor_SetsTools()
    {
        var openLibrary = new OpenLibraryService(
            Substitute.For<IHttpClientFactory>(),
            Substitute.For<ILogger<OpenLibraryService>>());
        var options = new GenericChatOptions(openLibrary);

        Assert.That(options.Tools, Is.Not.Null);
        Assert.That(options.Tools, Has.Count.EqualTo(1));
    }

    [Test]
    public void Constructor_ToolIsSearchBooks()
    {
        var openLibrary = new OpenLibraryService(
            Substitute.For<IHttpClientFactory>(),
            Substitute.For<ILogger<OpenLibraryService>>());
        var options = new GenericChatOptions(openLibrary);

        var tool = options.Tools![0];
        Assert.That(tool.Name, Is.EqualTo("search_books"));
    }
}
