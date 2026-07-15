using Core.Services;
using Microsoft.Extensions.AI;

namespace Core.Infrastructure;

/// <summary>
/// The chatoptions used by calls by IChatClient, this is where we'll register tools
/// </summary>
public class GenericChatOptions : ChatOptions
{
    public GenericChatOptions(OpenLibraryService openLibraryService)
    {
        var searchTool = AIFunctionFactory.Create(
            openLibraryService.SearchBooksAsync,
            name: "search_books",
            description: "Search books");
        
        Tools = [searchTool];
    }
}