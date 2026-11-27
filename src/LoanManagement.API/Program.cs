using System.Text;
using LoanManagement.API.Filters;
using LoanManagement.API.Middlewares;
using LoanManagement.Application;
using LoanManagement.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ExceptionHandlingMiddleware>();
builder.Services.AddScoped<IdempotencyMiddleware>();
builder.Services.AddScoped<TenantAuthorizationFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<TenantAuthorizationFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "DEV_ONLY_SUPER_SECRET_KEY_CHANGE_ME_1234567890")),
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddAuthorization(options =>
{
    foreach (var role in new[] { "LoanOfficer", "Underwriter", "Compliance", "BackOffice", "Accountant", "Collector", "RiskAnalyst", "Customer", "Admin", "Ops", "Support", "Security", "Scheduler", "System" })
    {
        options.AddPolicy(role, p => p.RequireRole(role));
    }
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Loan Management API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Bearer token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
    c.AddSecurityDefinition("Idempotency-Key", new OpenApiSecurityScheme
    {
        Name = "Idempotency-Key",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Required for mutating financial operations"
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Loan Management API v1");
    c.RoutePrefix = string.Empty; // Serves Swagger UI at root (http://localhost:5164/)
});
app.MapGet("/swagger", () => Results.Redirect("/"));
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<IdempotencyMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
