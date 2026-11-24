using Cart.API.Extensions;
using Cart.Persistence.Contexts;
using Cart.Persistence.Repositories;
using Cart.Services;
using Cart.Services.Abstracts;
using Cart.Services.Concretes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CartContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CartCampaingContext") ?? throw new InvalidOperationException("Connection string 'CartCampaingContext' not found.")));

builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddScoped(typeof(ICrudService<,,,,>), typeof(CrudService<,,,,>));
builder.Services.AddCrudServices(typeof(CrudService<,,,,>).Assembly);
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MapProfile).Assembly));

// JWT Bazlı Kimlik Doğrulama (IdentityApp'in ürettiği JWT çerezi ile çalışır)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes("SuperSecretKeyForInterviewProjectsJwtAuth2026!");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "IdentityApp",
        ValidAudience = "InterviewProjects",
        IssuerSigningKey = new SymmetricSecurityKey(key),
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };

    // Çerezdeki (Cookie) token'ı okuma entegrasyonu
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // 1. Önce "access_token" çerezine bak
            if (context.Request.Cookies.TryGetValue("access_token", out var cookieToken) && !string.IsNullOrEmpty(cookieToken))
            {
                context.Token = cookieToken;
            }
            // 2. Çerez yoksa standart Authorization: Bearer başlığına bakar
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(opt =>
{
    opt.AllowAnyHeader();
    opt.AllowAnyMethod();
    opt.SetIsOriginAllowed(origin => true);
    opt.AllowCredentials();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
