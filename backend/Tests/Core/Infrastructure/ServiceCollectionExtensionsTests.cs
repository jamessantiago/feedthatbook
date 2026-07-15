using Core;
using Core.Infrastructure;
using Core.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Core.Infrastructure;

public class InfrastructureServiceCollectionExtensionsTests
{
    [SetUp]
    public void Setup()
    {
        CoreGlobal.Settings = new Settings { GeminiApiKey = "test-key", GeminiModelId = "test-model" };
    }

    [TearDown]
    public void Teardown()
    {
        CoreGlobal.Settings = new Settings();
    }

    [Test]
    public void AddInfrastructure_RegistersChatOptions()
    {
        var services = new ServiceCollection();
        services.AddHttpClient();
        services.AddCoreServices();
        services.AddInfrastructure();
        var provider = services.BuildServiceProvider();

        var chatOptions = provider.GetService<ChatOptions>();
        Assert.That(chatOptions, Is.Not.Null);
    }

    [Test]
    public void AddInfrastructure_RegistersChatClientFactory()
    {
        var services = new ServiceCollection();
        services.AddInfrastructure();
        var provider = services.BuildServiceProvider();

        var factory = provider.GetService<IChatClientFactory>();
        Assert.That(factory, Is.Not.Null);
        Assert.That(factory, Is.TypeOf<GeminiChatClientFactory>());
    }

    [Test]
    public void AddInfrastructure_RegistersLlmService()
    {
        var services = new ServiceCollection();
        services.AddHttpClient();
        services.AddCoreServices();
        services.AddInfrastructure();
        var provider = services.BuildServiceProvider();

        var llm = provider.GetService<ILlmService>();
        Assert.That(llm, Is.Not.Null);
        Assert.That(llm, Is.TypeOf<LlmService>());
    }
}
