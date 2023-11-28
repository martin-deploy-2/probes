using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<MartinProbesAppSettings>()
    .Bind(builder.Configuration.GetSection("Martin.Probes"))
    .ValidateDataAnnotations();

var app = builder.Build();

// The startup probes blocks the other probes while the application cold-boots.
// If this HTTP endpoint is able to return, then the application has started,
// obviously.
app.MapGet("/api/v1/health/startup", () => "Started, obviously.");

// The liveness probe indicates whether the application itself is healthy and
// has correct configuration. Each framework will have its own way of checking
// the application configuration at startup.
//
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-7.0#options-validation
app.MapGet("/api/v1/health/liveness", (IOptionsSnapshot<MartinProbesAppSettings> options, HttpContext context) =>
{
    if (options.Value.INeedAnOptionToValidate)
    {
        return "Alive.";
    }
    else
    {
        context.Response.StatusCode  = 503;
        return "Dead.";
    }
});

// The readiness probe indicates whether the application AND its dependencies are healthy and ready to serve requests.
app.MapGet("/api/v1/health/readiness", () => "Readiness endpoint.");

app.Run();

public class MartinProbesAppSettings
{
    [Required]
    public bool INeedAnOptionToValidate { get; set; }
}
