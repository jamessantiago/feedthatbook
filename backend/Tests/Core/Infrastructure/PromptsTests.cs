using Core.Infrastructure;

namespace Tests.Core.Infrastructure;

public class PromptsTests
{
    [Test]
    public void ExtractFields_IsNotEmpty()
    {
        Assert.That(Prompts.ExtractFields, Is.Not.Empty);
    }

    [Test]
    public void ExtractFields_ContainsInstructions()
    {
        Assert.That(Prompts.ExtractFields, Does.Contain("extract structured fields"));
    }

    [Test]
    public void GenerateExplanation_IsNotEmpty()
    {
        Assert.That(Prompts.GenerateExplanation, Is.Not.Empty);
    }

    [Test]
    public void GenerateExplanation_ContainsInstructions()
    {
        Assert.That(Prompts.GenerateExplanation, Does.Contain("why this book"));
    }
}
