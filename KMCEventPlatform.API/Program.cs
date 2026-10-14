using System.Text;
using KMCEventPlatform.API.Data;
using KMCEventPlatform.API.Interfaces;
using KMCEventPlatform.API.Models;
using KMCEventPlatform.API.Repositories;
using KMCEventPlatform.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"
        )
    )
);

// Repository registrations.
builder.Services.AddScoped<
    IEventTypeRepository,
    EventTypeRepository
>();

builder.Services.AddScoped<
    IUserRepository,
    UserRepository
>();

builder.Services.AddScoped<
    IEventRepository,
    EventRepository
>();

builder.Services.AddScoped<
    IRegistrationRepository,
    RegistrationRepository
>();

// Service registrations.
builder.Services.AddScoped<IAuthService, AuthService>();

// Password hashing service.
builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>
>();

var jwtSection = builder.Configuration.GetSection("Jwt");

string jwtKey = jwtSection["Key"]
    ?? throw new InvalidOperationException(
        "JWT key is missing from configuration."
    );

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)
                    ),

                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Authentication must come before authorization.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();