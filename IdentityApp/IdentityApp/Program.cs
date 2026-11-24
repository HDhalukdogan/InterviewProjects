using IdentityApp.Consumers;
using IdentityApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddDbContext<IdentityAppContext>(option =>
{
    option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
 ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6389"));


builder.Services.AddIdentityApiEndpoints<IdentityUser>(opt =>
{
    opt.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<IdentityAppContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    // 1. HttpOnly: JavaScript (XSS) ile çerezin çalınmasını engeller, sadece tarayıcı yönetir.
    options.Cookie.HttpOnly = true;

    // 2. SameSite: Cross-Site Request Forgery (CSRF) koruması ve localhost portlar arası geçiş dengesi.
    options.Cookie.SameSite = SameSiteMode.Lax;

    // 3. SecurePolicy: HTTP ortamında (localhost) çerezin düşmemesini sağlar, HTTPS'de otomatik güvenli olur.
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

    // 4. OnRedirectToLogin: API mimarisinde unauthenticated isteklerde MVC gibi login sayfasına yönlendirmek (302) yerine 401 Unauthorized döner.
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };

    // 5. OnRedirectToAccessDenied: Yetkisiz rol/erişimlerde (Forbidden) yönlendirme (302) yerine 403 döner.
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

builder.Services.AddHostedService<UserConsumer>();

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
app.MapGroup("api").MapIdentityApi<IdentityUser>();

app.MapPost("api/logout", async (SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok(new { message = "Logged out successfully" });
});

app.Run();
