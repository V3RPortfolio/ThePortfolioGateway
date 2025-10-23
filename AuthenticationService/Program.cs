using AuthenticationService;
using System.IO;

Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder =>
{

    var coreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    var applicationHost = Environment.GetEnvironmentVariable("APPLICATION_HOST");
    if (applicationHost == null) applicationHost = "http://localhost:8005";

    var isDevelopment = coreEnvironment != null && coreEnvironment.ToLower() == "development";
    builder.UseStartup<Startup>();


    builder.UseUrls(applicationHost);

    builder.ConfigureAppConfiguration(config =>
    {
        // Open ocelot.json and update the application host dynamically
        var ocelotBaseUrlRegexpattern = "\"BaseUrl\":  \".*/api/v1\"";
        var regex = new System.Text.RegularExpressions.Regex(ocelotBaseUrlRegexpattern);

        var ocelotJsonPath = Path.Combine(Directory.GetCurrentDirectory(), "ocelot.json");
        var ocelotJsonContent = File.ReadAllText(ocelotJsonPath);
        var updatedOcelotJsonContent = regex.Replace(ocelotJsonContent, $"\"BaseUrl\":  \"{applicationHost}/api/v1\"");
        File.WriteAllText(ocelotJsonPath, updatedOcelotJsonContent);

        config.AddJsonFile("ocelot.json");

        if (!isDevelopment)
        {
            config.AddJsonFile("appsettings.json");
        }
        else
        {
            config.AddJsonFile("appsettings.Development.json");
        }
    });
}).Build().Run();