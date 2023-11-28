using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

var INeedAVariableToSimulateTheAvailabilityOfSomeFictitiousDatabase = true;

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
app.MapGet("/api/v1/health/liveness", (IOptionsSnapshot<MartinProbesAppSettings> options, HttpResponse response) =>
{
    if (options.Value.INeedAnOptionToValidate)
    {
        return "Alive.";
    }
    else
    {
        response.StatusCode  = 503;
        return "Dead.";
    }
});

// The readiness probe indicates whether the application AND its dependencies
// are healthy and ready to serve requests. I could duplicate the same tests as
// for liveness here, or even refactor the liveness endpoint into a function
// and call it from this endpoint, but the liveness endpoint will be probed
// separately anyways, so I don't see the point of checking for liveness again
// in the readiness endpoint.
app.MapGet("/api/v1/health/readiness", (HttpResponse response) =>
{
    if (INeedAVariableToSimulateTheAvailabilityOfSomeFictitiousDatabase)
    {
        return "Ready.";
    }
    else
    {
        response.StatusCode  = 503;
        return "Readon't."; // Have you noticed this little play on words? Humor is a sign of intelligence, I tell you.
    }

});

// An endpoint to simulate (by that I mean toggle) the availability of the
// fictitious database.
app.MapPost("/api/v1/health/db-availablility", () =>
{
    INeedAVariableToSimulateTheAvailabilityOfSomeFictitiousDatabase = !INeedAVariableToSimulateTheAvailabilityOfSomeFictitiousDatabase;

    if (INeedAVariableToSimulateTheAvailabilityOfSomeFictitiousDatabase)
    {
        return "Now available.";
    }
    else
    {
        return "Now unavailable.";
    }

});

app.Run();

public class MartinProbesAppSettings
{
    [Required]
    public bool INeedAnOptionToValidate { get; set; }
}
