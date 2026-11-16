using AppointmentDbFirst.Api.Consumers;
using AppointmentDbFirst.Infrastructure.RabbitMQ;
using AppointmentDbFirst.Infrastructure.Redis;
using AppointmentDbFirst.Persistence.Contexts;
using AppointmentDbFirst.Service.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppointmentDbFirstContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppointmentDbFirstContext") ?? throw new InvalidOperationException("Connection string 'AppointmentDbFirstContext' not found.")));
//builder.Services.AddDbContext<AppointmentDbFirstContext>();

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisDb") ?? throw new InvalidOperationException("Connection string 'RedisDb' not found.")));

builder.Services.AddHostedService<AppointmentConsumer>();

builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();

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
