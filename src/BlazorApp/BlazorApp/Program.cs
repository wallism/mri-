using BlazorApp;
using BlazorApp.Data;
using Common.Utilities;
using Dapr.Client;
using Serilog;
using Serilog.Events;

try
{
    var builder = WebApplication.CreateBuilder(args);

    var lc = new LoggerConfiguration();
    ServiceStartup.ConfigureLogger(lc, builder.Configuration);
    Log.Logger = lc.CreateLogger();
    builder.Logging.AddSerilog();

    var config = builder.Configuration;
    // in PROD console should be set to Error
    Enum.TryParse(config["Serilog:SinkLevel:Console"], out LogEventLevel consoleLevel);
    Enum.TryParse(config["Serilog:SinkLevel:SumoLogic"], out LogEventLevel sumoLogicLevel);
    builder.Services.AddDaprClient();


    builder.Services.AddHttpClient<IPublicApiService, PublicApiService>(c => 
        new PublicApiService(DaprClient.CreateInvokeHttpClient("public-api")));

    // Add services to the container.
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
    builder.Services.AddSingleton<WeatherForecastService>();
    builder.Services.AddSingleton<ISampleService, SampleService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseRouting();

    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}