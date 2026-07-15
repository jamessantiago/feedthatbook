using System.Collections.Concurrent;
using System.Threading.Channels;
using Core.DTOs;

namespace Api.Services;

public class SearchRequestStore
{
    private readonly ConcurrentDictionary<Guid, Channel<BookCandidateResponse>> _requests = new();

    public (Guid Id, Channel<BookCandidateResponse> Channel) Create()
    {
        var id = Guid.NewGuid();
        var channel = Channel.CreateBounded<BookCandidateResponse>(new BoundedChannelOptions(100));
        _requests.TryAdd(id, channel);
        return (id, channel);
    }

    public Channel<BookCandidateResponse>? Get(Guid id) => _requests.GetValueOrDefault(id);
}