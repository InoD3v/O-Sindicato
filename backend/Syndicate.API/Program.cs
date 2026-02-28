using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Syndicate.API.Middlewares;
using Syndicate.Infrastructure;
using Syndicate.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// â”€â”€ Controllers â”€â”€
builder.Services.AddControllers();

// â”€â”€ Swagger / OpenAPI â”€â”€
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "O Sindicato API",
        Version = "v1",
        Description = "API for group management with social currency (Pikas)"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});

// â”€â”€ MediatR (scan Domain assembly for handlers) â”€â”€
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Syndicate.Domain.Common.Result).Assembly);
    cfg.AddOpenBehavior(typeof(Syndicate.Domain.Features.ValidationBehavior<,>));
});

// â”€â”€ FluentValidation (scan Domain assembly for validators) â”€â”€
builder.Services.AddValidatorsFromAssembly(typeof(Syndicate.Domain.Common.Result).Assembly);

// â”€â”€ Infrastructure (EF Core + Repositories) â”€â”€
builder.Services.AddInfrastructure(builder.Configuration);

// â”€â”€ JWT Authentication â”€â”€
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSection["Secret"]!))
        };
    });
builder.Services.AddAuthorization();

// â”€â”€ CORS â”€â”€
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();
// â"€â"€ Auto-migrate in Development â"€â"€
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<SyndicateDbContext>();
    db.Database.Migrate();
}
// â”€â”€ Middleware Pipeline â”€â”€
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
