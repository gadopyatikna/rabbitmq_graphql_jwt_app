using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using ProtectedService.Configs;

IdentityModelEventSource.ShowPII = true;

// Secret key for JWT
var secretKey = "my-very-long-token-suoer-puper-secret-key";

var builder = WebApplication.CreateBuilder(args);

var rabbitMqHost = builder.Configuration.GetValue<string>("RabbitMqHost", "localhost");
var queueName = builder.Configuration.GetValue<string>("QueueName", "task_queue");

builder.Services.Configure<RabbitMqConfig>(_ => new RabbitMqConfig() { RabbitMqHost = rabbitMqHost, QueueName = queueName});

// Add HostedService with parameters passed in the constructor
builder.Services.AddHostedService(sp => new RabbitMqConsumer.RabbitMqConsumer(rabbitMqHost, queueName));

// Add JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.UseSecurityTokenValidators = true;
    options.IncludeErrorDetails = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
// Add Swagger services
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddHostedService<RabbitMqConsumer.RabbitMqConsumer>();

var app = builder.Build();
app.UseSwagger();  // Enable Swagger
app.UseSwaggerUI();  // Enable Swagger UI

// Use authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();