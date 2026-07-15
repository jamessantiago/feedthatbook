using System.Text.Json.Serialization;

namespace Core.DTOs;

public class BookCandidate
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("first_publish_year")]
    public int? FirstPublishedYear { get; set; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }
}

public class BookCandidateResponse
{
    [JsonPropertyName("matches")]
    public List<BookCandidate>? Matches { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; } = true;
    
    [JsonPropertyName("error")]
    public string? Error { get; set; }
}