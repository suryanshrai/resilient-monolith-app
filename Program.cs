using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Try reading connection string from multiple sources
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

// Root endpoint
app.MapGet("/", () => "Resilient Monolith CI/CD Pipeline Working!");

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

app.MapGet("/db-test", async () =>
{
    if (string.IsNullOrEmpty(connectionString))
        return Results.Problem("Connection string not configured.");

    try
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        return Results.Ok("Database connection successful!");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection failed: {ex.Message}");
    }
});

app.Run();