using Core.DTOs;

namespace Tests.Core.DTOs;

public class BookCandidateTests
{
    [Test]
    public void Constructor_DefaultValuesAreNull()
    {
        var c = new BookCandidate();
        Assert.Multiple(() =>
        {
            Assert.That(c.Title, Is.Null);
            Assert.That(c.Author, Is.Null);
            Assert.That(c.FirstPublishedYear, Is.Null);
            Assert.That(c.Explanation, Is.Null);
        });
    }

    [Test]
    public void CanSetProperties()
    {
        var c = new BookCandidate
        {
            Title = "The Hobbit",
            Author = "J.R.R. Tolkien",
            FirstPublishedYear = 1937,
            Explanation = "Exact title match"
        };
        Assert.Multiple(() =>
        {
            Assert.That(c.Title, Is.EqualTo("The Hobbit"));
            Assert.That(c.Author, Is.EqualTo("J.R.R. Tolkien"));
            Assert.That(c.FirstPublishedYear, Is.EqualTo(1937));
            Assert.That(c.Explanation, Is.EqualTo("Exact title match"));
        });
    }
}

public class BookCandidateResponseTests
{
    [Test]
    public void Constructor_Defaults()
    {
        var r = new BookCandidateResponse();
        Assert.Multiple(() =>
        {
            Assert.That(r.Matches, Is.Null);
            Assert.That(r.Success, Is.True);
        });
    }

    [Test]
    public void CanSetProperties()
    {
        var r = new BookCandidateResponse
        {
            Matches = [new BookCandidate { Title = "Test" }],
            Success = false
        };
        Assert.Multiple(() =>
        {
            Assert.That(r.Matches, Has.Count.EqualTo(1));
            Assert.That(r.Matches![0].Title, Is.EqualTo("Test"));
            Assert.That(r.Success, Is.False);
        });
    }
}
