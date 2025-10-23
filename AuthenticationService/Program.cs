using AuthenticationService;

Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder =>
{
    var environmentName = Environment.GetEnvironmentVariable("OCELOT_RUNTIME_CATEGORY");
    var isDocker = environmentName != null && environmentName.ToLower() == "docker";
    builder.UseStartup<Startup>();
    if(!isDocker) builder.UseUrls("http://localhost:8005");

    builder.ConfigureAppConfiguration(config =>
    {
        if(isDocker)
        {
            config.AddJsonFile("ocelot.docker.json");
            config.AddJsonFile("appsettings.docker.json");
        }
        else
        {
            config.AddJsonFile("ocelot.dev.json");
            config.AddJsonFile("appsettings.Development.json");
        }
    });
}).Build().Run();