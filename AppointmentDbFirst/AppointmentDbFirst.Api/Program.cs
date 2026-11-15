using AppointmentDbFirst.Persistence.Contexts;
using AppointmentDbFirst.Service.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppointmentDbFirstContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppointmentDbFirstContext") ?? throw new InvalidOperationException("Connection string 'AppointmentDbFirstContext' not found.")));

//builder.Services.AddDbContext<AppointmentDbFirstContext>();

builder.Services.AddScoped<IDoctorService, DoctorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
