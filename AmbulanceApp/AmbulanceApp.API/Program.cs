using AmbulanceApp.API.Consumers;
using AmbulanceApp.Persistence.Contexts;
using AmbulanceApp.Service.RabbitMQ;
using AmbulanceApp.Service.Redis;
using AmbulanceApp.Service.Services.Abstract;
using AmbulanceApp.Service.Services.Concrete;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// 1. PostgreSQL DbContext
builder.Services.AddDbContext<AmbulanceAppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Redis GEO & Distributed Lock Servisi (Singleton)
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(RedisConstants.ConnectionString));
builder.Services.AddSingleton<IRedisGeoLockService, RedisGeoLockService>();

// 3. RabbitMQ Servisi (Singleton) ve Audit Consumer (HostedService)
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
builder.Services.AddHostedService<AmbulanceDispatchConsumer>();

builder.Services.AddScoped<IAmbulanceService, AmbulanceService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
