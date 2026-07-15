using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Api.Services;
using Core.DTOs;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/books")]
public class Books : ControllerBase
{
    [HttpPost("search")]
    [EndpointSummary("Search for book results using AI")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
    public ActionResult<StartSearchResult> StartSearch(
        [FromServices] SearchRequestStore store,
        [FromServices] AiBookResolver resolver,
        [FromQuery] [MaxLength(500)] string query)
    {
        // register a channel
        var (id, channel) = store.Create();

        // kickoff the query in the background and return early as LLM performance is
        // unpredictable and may often violate a <500ms SLO
        _ = Task.Run(async () =>
        {
            var result = await resolver.ResolveBook(query);
            await channel.Writer.WriteAsync(result);
            channel.Writer.Complete();
        });
        
        return Ok(new StartSearchResult { RequestId = id });
    }
    
    [HttpGet("stream/{id:guid}")]
    [EndpointSummary("SSO stream for book results using AI")]
    public async Task Stream(
        Guid id,
        [FromServices] SearchRequestStore store,
        CancellationToken token)
    {
        var channel = store.Get(id);
        if (channel is null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        HttpContext.Response.ContentType = "text/event-stream";

        await foreach (var candidate in channel.Reader.ReadAllAsync(token))
        {
            var json = JsonSerializer.Serialize(candidate);
            await HttpContext.Response.WriteAsync($"data: {json}\n\n", token);
            await HttpContext.Response.Body.FlushAsync(token);
        }
    }
}