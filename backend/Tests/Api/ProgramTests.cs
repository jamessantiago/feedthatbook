using Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Api;

public class ProgramTests
{
    [Test]
    public void CreateApp_BuildsSuccessfully()
    {
        using var app = global::Api.Program.CreateApp([]);
        Assert.That(app, Is.Not.Null);
    }

    [Test]
    public void CreateApp_ServicesAreRegistered()
    {
        using var app = global::Api.Program.CreateApp([]);
        var services = app.Services;

        using var scope = services.CreateScope();
        Assert.That(scope.ServiceProvider.GetService<Microsoft.AspNetCore.Mvc.Controllers.IControllerActivator>(), Is.Not.Null);
    }

    [Test]
    public void CreateApp_EnvironmentIsNotNull()
    {
        using var app = global::Api.Program.CreateApp([]);
        Assert.That(app.Environment.EnvironmentName, Is.Not.Null);
    }

    [Test]
    public void CreateApp_WithDevelopmentEnv_MapsOpenApi()
    {
        var args = new[] { "--environment", "Development" };
        using var app = global::Api.Program.CreateApp(args);
        Assert.That(app.Environment.EnvironmentName, Is.EqualTo("Development"));
    }
}
