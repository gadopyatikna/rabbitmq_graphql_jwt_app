using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using ProtectedService.Configs;

IdentityModelEventSource.ShowPII = true;

var secretKey = "my-very-long-token-suoer-puper-secret-key";

var builder = WebApplication.CreateBuilder(args);
var rabbitMqHost = builder.Configuration.GetValue<string>("RabbitMqHost");
var queueName = builder.Configuration.GetValue<string>("QueueName");

builder.Services.Configure<RabbitMqConfig>(_ => new RabbitMqConfig() { RabbitMqHost = rabbitMqHost, QueueName = queueName});

builder.Services.AddHostedService(sp => new RabbitMqConsumer.RabbitMqConsumer(rabbitMqHost, queueName));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
        }
    );

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

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

var app = builder.Build();
app.UseSwagger();  
app.UseSwaggerUI();  

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();