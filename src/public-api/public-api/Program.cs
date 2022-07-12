using Common.Utilities;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;


try
{
    var builder = WebApplication.CreateBuilder(args);
    ServiceStartup.UseSerilogOnHost(builder);

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddDaprClient();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHttpContextAccessor();

    var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();

    // https://github.com/serilog/serilog-aspnetcore
    app.UseSerilogRequestLogging();

    app.UseAuthorization();
    app.MapControllers();

    app.UseExceptionHandler(
        options =>
        {
            options.Run(async context =>
            {
                context.Response.ContentType = "text/html";
                var exceptionObject = context.Features.Get<IExceptionHandlerFeature>();
                if (null != exceptionObject)
                {
                    var errorMessage = $"{exceptionObject.Error.Message}";
                    // todo: different error if running in prod
                    Log.Error(exceptionObject.Error, "unhandled"); // doesn't mean it should be handled
                    await context.Response.WriteAsync(errorMessage).ConfigureAwait(false);
                }
            });
        }
    );

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


 