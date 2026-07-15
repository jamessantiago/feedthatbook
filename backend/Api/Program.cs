using Api.Services;
using Core;
using Core.Infrastructure;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace Api;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateApp(args).Run();
    }

    public static WebApplication CreateApp(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);
        builder.Configuration.Bind(CoreGlobal.Settings);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddHttpClient();

        builder.Services.AddInfrastructure();
        builder.Services.AddCoreServices();
        builder.Services.AddApiServices();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        return app;
    }
}