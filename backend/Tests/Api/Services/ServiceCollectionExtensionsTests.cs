using Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Api.Services;

public class ApiServicesServiceCollectionExtensionsTests
{
    [Test]
    public void AddApiServices_RegistersSearchRequestStore()
    {
        var services = new ServiceCollection();
        services.AddApiServices();
        var provider = services.BuildServiceProvider();

        var store = provider.GetService<SearchRequestStore>();
        Assert.That(store, Is.Not.Null);
    }
}
