using DoctorRedis.API.Extensions;
using DoctorRedis.Persistence.Contexts;
using DoctorRedis.Service.DTOs;
using DoctorRedis.Service.Mappings;
using DoctorRedis.Service.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Net.Sockets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors();

builder.Services.AddDbContext<DoctorRedisContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DoctorRedisApp") ?? throw new InvalidOperationException("Connection string 'DoctorRedisApp' not found.")));

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisDb") ?? throw new InvalidOperationException("Connection string 'RedisDb' not found.")));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<DoctorCreateDtoValidator>();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
//builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

builder.Services.AddIocRegistration();

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
    opt.AllowAnyOrigin();
});

app.UseAuthorization();

app.MapControllers();

app.Run();


//docker run -d --name rabbitmq-interview -p 5682:5672 -p 15682:15672 rabbitmq:3-management

//docker run -d --name redis-interview -p 6389:6379 redis:latest