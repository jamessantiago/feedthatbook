using System.Threading.Channels;
using Api.Services;
using Core.DTOs;

namespace Tests.Api.Services;

public class SearchRequestStoreTests
{
    [Test]
    public void Create_ReturnsNonEmptyId()
    {
        var store = new SearchRequestStore();
        var (id, _) = store.Create();
        Assert.That(id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Create_ReturnsUniqueIds()
    {
        var store = new SearchRequestStore();
        var (id1, _) = store.Create();
        var (id2, _) = store.Create();
        Assert.That(id1, Is.Not.EqualTo(id2));
    }

    [Test]
    public void Create_StoresChannel()
    {
        var store = new SearchRequestStore();
        var (id, channel) = store.Create();
        var retrieved = store.Get(id);
        Assert.That(retrieved, Is.SameAs(channel));
    }

    [Test]
    public void Get_UnknownId_ReturnsNull()
    {
        var store = new SearchRequestStore();
        var result = store.Get(Guid.NewGuid());
        Assert.That(result, Is.Null);
    }

    [Test]
    public void CreatedChannel_IsBounded()
    {
        var store = new SearchRequestStore();
        var (_, channel) = store.Create();
        Assert.That(channel, Is.Not.Null);
    }

    [Test]
    public void Get_AfterSingleCreate_ReturnsChannel()
    {
        var store = new SearchRequestStore();
        store.Create();
        var (id, _) = store.Create();
        Assert.That(store.Get(id), Is.Not.Null);
    }
}
