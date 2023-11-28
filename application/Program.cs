var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/v1/health/startup", () => "Startup endpoint.");
app.MapGet("/api/v1/health/liveness", () => "Liveness endpoint.");
app.MapGet("/api/v1/health/readyness", () => "Readyness endpoint.");

app.Run();
