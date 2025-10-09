using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

//------------------------------------------------------------------------------------------------
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Middleware to validate tokens sent by the API clients:
app.Use(async (context, next) => {
    // This apply onlt if there are /api routes:
    if (context.Request.Path.StartsWithSegments("/api")) {
        // Get the defined token in the settings file:
        var tokenConfig = builder.Configuration["TokenConfig:Secret"];

        // Verify if the client sent the authorization header:
        if (!context.Request.Headers.TryGetValue("Authorization", out var token) ||
            token != $"Bearer {tokenConfig}") {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new { error = "Forbidden: Missing or invalid token" });
            return;
        }
    }

    await next();
});
