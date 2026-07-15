using Core.DTOs;

namespace Tests.Core.DTOs;

public class StartSearchResultTests
{
    [Test]
    public void DefaultRequestIdIsEmpty()
    {
        var r = new StartSearchResult();
        Assert.That(r.RequestId, Is.EqualTo(Guid.Empty));
    }

    [Test]
    public void CanSetRequestId()
    {
        var id = Guid.NewGuid();
        var r = new StartSearchResult { RequestId = id };
        Assert.That(r.RequestId, Is.EqualTo(id));
    }
}
