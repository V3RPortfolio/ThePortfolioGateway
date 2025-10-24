using AuthenticationService;
using Infrastructure.Services;
using Ocelot.Configuration.File;
using Ocelot.DependencyInjection;
using System.IO;


Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder =>
{

    var coreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    var applicationHost = Environment.GetEnvironmentVariable("APPLICATION_HOST") ?? "http://localhost";

    var isDocker = (Environment.GetEnvironmentVariable("IS_DOCKER_ENVIRONMENT") ?? "false").ToLower() == "true";
    builder.UseStartup<Startup>();


    builder.UseUrls(applicationHost);

    builder.ConfigureAppConfiguration(config =>
    {
        var appSettingsFile = isDocker ? "appsettings.Docker.json" : "appsettings.Standalone.json";
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile(appSettingsFile, optional: false, reloadOnChange: true);
        new OcelotConfiguration().Configure(config, appSettingsFile, applicationHost);
    });
}).Build().Run();