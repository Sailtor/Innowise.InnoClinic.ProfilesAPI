using InnoClinic.ProfilesAPI.Extensions;
using Serilog;

LoggingExtensions.CongigureLogger();

try
{
    Log.Information("Starting web host");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console());

    // Add services to the container.
    builder.Services.ConfigureControllers();
    builder.Services.ConfigureSwagger();
    builder.Services.ConfigureEntityServices();
    builder.Services.ConfigureMsSqlServerContext(builder.Configuration);
    builder.Services.ConfigureAutomapper();
    builder.Services.CofigureAuthorization();
    builder.Services.ConfigureOwnerAuthZPolicies();
    builder.Services.ConfigureRabbitMQConsumer();

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseExceptionHandlerMiddleware();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    /*add logging and exception handling middleware later*/
    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}