using Core;
using Core.Infrastructure;

namespace Tests.Core.Infrastructure;

public class ChatClientFactoryTests
{
    [SetUp]
    public void Setup()
    {
        CoreGlobal.Settings = new Settings();
    }

    [Test]
    public void GeminiChatClientFactory_NoApiKey_Throws()
    {
        var factory = new GeminiChatClientFactory();
        Assert.That(() => factory.CreateClient(), Throws.ArgumentNullException);
    }

    [Test]
    public void GeminiChatClientFactory_WithApiKey_ReturnsClient()
    {
        CoreGlobal.Settings.GeminiApiKey = "test-api-key";
        CoreGlobal.Settings.GeminiModelId = "test-model";

        var factory = new GeminiChatClientFactory();
        var client = factory.CreateClient();

        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void GeminiChatClientFactory_EmptyApiKey_Throws()
    {
        CoreGlobal.Settings.GeminiApiKey = "";
        var factory = new GeminiChatClientFactory();
        Assert.That(() => factory.CreateClient(), Throws.ArgumentNullException);
    }

    [Test]
    public void GeminiChatClientFactory_WhitespaceApiKey_Throws()
    {
        CoreGlobal.Settings.GeminiApiKey = "   ";
        var factory = new GeminiChatClientFactory();
        Assert.That(() => factory.CreateClient(), Throws.ArgumentNullException);
    }

    [Test]
    public void GeminiChatClientFactory_ApiKeySet_EmptyModelId_Throws()
    {
        CoreGlobal.Settings.GeminiApiKey = "test-key";
        CoreGlobal.Settings.GeminiModelId = "";
        var factory = new GeminiChatClientFactory();
        Assert.That(() => factory.CreateClient(), Throws.ArgumentNullException);
    }

    [Test]
    public void GeminiChatClientFactory_ApiKeySet_WhitespaceModelId_Throws()
    {
        CoreGlobal.Settings.GeminiApiKey = "test-key";
        CoreGlobal.Settings.GeminiModelId = "   ";
        var factory = new GeminiChatClientFactory();
        Assert.That(() => factory.CreateClient(), Throws.ArgumentNullException);
    }

    [Test]
    public void GeminiChatClientFactory_ApiKeySet_NullModelId_Throws()
    {
        CoreGlobal.Settings.GeminiApiKey = "test-key";
        CoreGlobal.Settings.GeminiModelId = null!;
        var factory = new GeminiChatClientFactory();
        Assert.That(() => factory.CreateClient(), Throws.ArgumentNullException);
    }

    [TearDown]
    public void Teardown()
    {
        CoreGlobal.Settings = new Settings();
    }
}
