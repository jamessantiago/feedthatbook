using Core;
using Core.Infrastructure;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Core.Services;

public class CoreServicesServiceCollectionExtensionsTests
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
    public void AddCoreServices_RegistersOpenLibraryService()
    {
        var services = new ServiceCollection();
        services.AddHttpClient();
        services.AddCoreServices();
        var provider = services.BuildServiceProvider();

        var service = provider.GetService<OpenLibraryService>();
        Assert.That(service, Is.Not.Null);
    }

    [Test]
    public void AddCoreServices_RegistersAiBookResolver()
    {
        var services = new ServiceCollection();
        services.AddHttpClient();
        services.AddCoreServices();
        services.AddInfrastructure();
        var provider = services.BuildServiceProvider();

        var service = provider.GetService<AiBookResolver>();
        Assert.That(service, Is.Not.Null);
    }
}
