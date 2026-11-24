using IdentityApp.Consumers;
using IdentityApp.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Shared Data Protection configuration so other microservices (e.g. DoctorAppointmentBookingAPI) can decrypt cookies
var sharedKeysDirectory = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ASP.NET", "DataProtection-Keys"));
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(sharedKeysDirectory)
    .SetApplicationName("InterviewProjectsSharedApp");

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

    // 4. JWT Token Cookie Entegrasyonu: Giriş yapıldığında Cart.API gibi JWT bazlı mikroservisler için "access_token" çerezi üretir
    options.Events.OnSigningIn = context =>
    {
        if (context.Principal != null)
        {
            var jwt = GenerateJwtToken(context.Principal);
            context.Response.Cookies.Append("access_token", jwt, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = context.Request.IsHttps,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
        }
        return Task.CompletedTask;
    };

    options.Events.OnSigningOut = context =>
    {
        context.Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
        return Task.CompletedTask;
    };

    // 5. OnRedirectToLogin: API mimarisinde unauthenticated isteklerde MVC gibi login sayfasına yönlendirmek (302) yerine 401 Unauthorized döner.
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };

    // 6. OnRedirectToAccessDenied: Yetkisiz rol/erişimlerde (Forbidden) yönlendirme (302) yerine 403 döner.
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

app.MapPost("api/logout", async (SignInManager<IdentityUser> signInManager, HttpContext httpContext) =>
{
    await signInManager.SignOutAsync();
    httpContext.Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
    return Results.Ok(new { message = "Logged out successfully" });
});

app.MapGet("api/token", (HttpContext httpContext) =>
{
    if (httpContext.Request.Cookies.TryGetValue("access_token", out var token) && !string.IsNullOrEmpty(token))
    {
        return Results.Ok(new { token });
    }
    return Results.Unauthorized();
});

app.Run();

// JWT Üretici Yardımcı Metot
static string GenerateJwtToken(ClaimsPrincipal principal)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes("SuperSecretKeyForInterviewProjectsJwtAuth2026!");

    var claims = new List<Claim>();
    var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var email = principal.FindFirst(ClaimTypes.Email)?.Value ?? principal.Identity?.Name;
    var name = principal.Identity?.Name;

    if (!string.IsNullOrEmpty(userId))
        claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
    if (!string.IsNullOrEmpty(email))
        claims.Add(new Claim(ClaimTypes.Email, email));
    if (!string.IsNullOrEmpty(name))
        claims.Add(new Claim(ClaimTypes.Name, name));

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddDays(7),
        Issuer = "IdentityApp",
        Audience = "InterviewProjects",
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}
