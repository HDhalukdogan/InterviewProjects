using CartDbFirstApp.API.Consumers;
using CartDbFirstApp.Persistence.Contexts;
using CartDbFirstApp.Service.CacheServices;
using CartDbFirstApp.Service.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<CartDbFirstAppContext>();
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect("localhost:6389"));
builder.Services.AddMemoryCache();
builder.Services.AddKeyedScoped<ICacheService, MemoryCacheService>("memory");
builder.Services.AddKeyedScoped<ICacheService, RedisCacheService>("redis");
builder.Services.AddHostedService<CartConsumer>();


builder.Services.AddScoped<ICartService, CartService>();


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
