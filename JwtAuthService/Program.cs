using JwtAuthService.Services;
using Microsoft.IdentityModel.Logging;

IdentityModelEventSource.ShowPII = true;
var builder = WebApplication.CreateBuilder(args);

// Secret key for JWT
var secretKey = "my-very-long-token-suoer-puper-secret-key";

// Add services
builder.Services.AddSingleton(new AuthService(secretKey));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
// Add Swagger services
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();  // Enable Swagger
app.UseSwaggerUI();  // Enable Swagger UI

// Use authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();