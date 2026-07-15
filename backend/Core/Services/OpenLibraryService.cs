using Microsoft.Extensions.Logging;

namespace Core.Services;

/// <summary>
/// Queries the Open Library's search API for book results (see https://openlibrary.org/dev/docs/api/search)
/// </summary>
public class OpenLibraryService(IHttpClientFactory httpClientFactory, ILogger<OpenLibraryService> logger)
{
    private const string SearchUrl = "https://openlibrary.org/search.json";

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(OpenLibraryService));

    public Task<string> SearchBooksAsync(string query, CancellationToken token = default)
    {
        logger.LogInformation("LlmService::SearchBooksAsync {query}", query);
        var url = $"{SearchUrl}?q={Uri.EscapeDataString(query)}&lang=en&fields=key,title,author_name,editions,editions.key,editions.title,editions.language,editions.cover_i";
        return _httpClient.GetStringAsync(url, token);
    }
}