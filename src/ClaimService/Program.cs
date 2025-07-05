using Amazon.Rekognition;
using Amazon.S3;
using Amazon.SQS;
using ClaimService.Configurations;
using ClaimService.External.Config;
using ClaimService.Middlewares;
using ClaimService.Services;
using ClaimService.Services.Documents;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Formatting.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logConfig =>
{
    logConfig.ClearProviders();
    logConfig.AddDebug();
    logConfig.AddEventSourceLogger();
    ConfigureLogging(builder);
    logConfig.AddSerilog();
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Jwt:SecretKey"))),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer"),
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience"),
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
builder.Services.AddExternalServices(builder.Configuration);
builder.Services.AddUserClient(builder.Configuration);
AddSwaggerGen(builder);
builder.Services.AddUsersDBContext(builder.Configuration);
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddAWSService<IAmazonRekognition>();
builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.AddScoped<IClaimService, ClaimsService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IFileUploader>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var useS3 = config.GetValue<bool>("FileStorage:UseS3");

    if (useS3)
    {
        var s3Client = sp.GetRequiredService<IAmazonS3>();
        return new S3FileUploader(s3Client, config);
    }
    else
    {
        var env = sp.GetRequiredService<IWebHostEnvironment>();
        return new LocalFileUploadService(env);
    }
});
builder.Services.AddScoped<IEventPublisher, EventPublisher>();
var app = builder.Build();
var env = builder.Environment.EnvironmentName?.ToLower();
builder.Configuration.AddJsonFile("appsettings.json", false, true);
if (env == "local")
{
    builder.Configuration.AddJsonFile($"appsettings.local.json", false, true);
}
else
{
    builder.Configuration.AddJsonFile($"appsettings.dev.json", false, true);
}
// Configure the HTTP request pipeline.
app.UseCors(policy => policy
.AllowAnyMethod()
.AllowAnyHeader()
.AllowAnyOrigin()
);
var basePath = "/claim-service";
// Ensure correct path handling in ALB
app.UsePathBase(new PathString(basePath));
app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionHandler>();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
    c.SwaggerEndpoint($"{basePath}/swagger/v1/swagger.json", "ClaimService V1");
});
app.UseRouting();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();

static void ConfigureLogging(WebApplicationBuilder builder)
{
    //Configure serilog as logger
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console(new JsonFormatter(renderMessage: false))
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers())
        .Enrich.With(new ExceptionEnricher())
        .CreateLogger();
    builder.Host.UseSerilog();
}

static void AddSwaggerGen(WebApplicationBuilder builder)
{
    builder.Services.AddSwaggerGen(c =>
    {
        //Add JWT Bearer Authentication to Swagger UI
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer <your-token>' below. Example: Bearer eyJhbGciOi..."
        });

        //Apply Authentication Globally
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
}

class ExceptionEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Exception == null)
            return;

        var logEventProperty = propertyFactory.CreateProperty("EscapedException", logEvent.Exception.ToString().Replace("\r\n", "\\r\\n"));
        logEvent.AddPropertyIfAbsent(logEventProperty);
    }
}
