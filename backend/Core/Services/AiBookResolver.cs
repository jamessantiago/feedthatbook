using Core.Infrastructure;
using System.Text.Json;
using Core.DTOs;
using Microsoft.Extensions.Logging;

namespace Core.Services;

/// <summary>
/// LLM powered service to return a structured book result list of candidates given a book search term 
/// </summary>
public class AiBookResolver(ILlmService llmService, ILogger<AiBookResolver> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<BookCandidateResponse> ResolveBook(string searchQuery, CancellationToken token = default)
    {
        string response;
        try
        {
            response = await llmService.OneShotAsync(Prompt, searchQuery, token);
            if (string.IsNullOrEmpty(response))
            {
                return new BookCandidateResponse { Success = false, Error = "Empty result returned"};
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to resolve book");
            return  new BookCandidateResponse { Success = false,  Error = ex.Message };
        }

        try
        {
            var result = JsonSerializer.Deserialize<BookCandidateResponse>(response, JsonOptions);
            if (result != null) return result;
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize results");
            return new BookCandidateResponse { Success = false, Error = $"Failed to deserialize results:\n{response}" };
        }

        return new BookCandidateResponse { Success = false, Error = $"Empty result returned from result:\n{response}" };
    }

    private const string Prompt = """
You are a book discovery assistant. You have access to the `search_books` tool which searches the Open Library catalog.

Follow these steps for each query:

1. **Extract** structured fields from the messy input (title, author, keywords).
2. **Search** -- call `search_books` with the best query you can construct from those fields.
3. **Review** the results and identify the best match using this hierarchy:
- Exact/normalized title + primary author (strongest)
- Exact/normalized title + contributor-only author (lower rank)
- Near-match title + author
- Author-only (return top works by that author)
4. **Prioritize** primary authors over contributors (illustrators, editors, adaptors).
5. **If no clear winner**, return up to 5 ordered candidates.

For each candidate return:
{
"title": string,
"author": string,
"first_publish_year": int,
"explanation": string,
"summary": string,
"link": string,
"img_link": string,
}

The `explanation` field must explain WHY this book matched the query -- cite which fields aligned (title, author), what rank in the hierarchy was used, and whether the author is a primary author or a contributor. Do NOT describe the book's plot or content.

Examples:
- "Exact title match; Tolkien is primary author; Dixon listed as adaptor."
- "Title near-match ('Two Cities' for 'A Tale of Two Cities'); Dickens is primary author."
- "Author-only match: top work by Stephen King."

The summary should specify the edition details.

The link field should prefer english results and use the openlibrary.org/books path.

Return a JSON array wrapped in { "matches": [...] }.
Do NOT include any text outside the JSON object.
""";
}
