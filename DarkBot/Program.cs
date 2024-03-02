using System.Text;
using DarkBot.Discord;
using DarkBot.Services;
using DarkBot.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Tailwind;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddSingleton(
    new HttpClient(
        new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
        })
    {
        BaseAddress = new Uri($"https://discord.com/api/v{builder.Configuration["apiVersion"]}"),
        DefaultRequestHeaders = { { "Authorization", $"Bot {builder.Configuration["botToken"]}" } }
    });

// Transients
builder.Services.AddTransient<QotService>();
builder.Services.AddTransient<BannerService>();
builder.Services.AddTransient<BotService>();


// The HttpClient in AccountService is different than in other services and needs to have manually assigned auth token to get the users info
builder.Services.AddHttpClient<AccountService>(client =>
{
    client.BaseAddress = new Uri($"https://discord.com/api/v{builder.Configuration["apiVersion"]}");
});

builder.Services.AddTransient<HomeService>();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddCookie()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["randomJwtToken"]!))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("account", out string token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorizationCore();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.RunTailwind("tailwind");
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseAuthentication();
app.UseRouting();

app.UseAntiforgery();
app.UseAuthorization();

app.MapDelete("api/logout", async (HttpContext context, AccountService service) =>
{
    context.Response.Cookies.Delete("account");
    var id = context.User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
    if (id is null)
        Results.Forbid();
        
    await service.DeleteAccount(id!);
    Results.NoContent();
});

app.MapGet("api/authorize", async (HttpContext context, [FromQuery] string code, AccountService service) =>
{
    var account = await service.Authenticate(code);

    if (account is null)
        Results.Forbid();

    var user = await service.GetUser(account!);

    if (user is null)
        Results.Forbid();

    account!.UserId = user!.Id;
    await service.SaveAccount(account);

    var jwtToken = service.CreateJwtToken(user);

    var options = new CookieOptions
    {
        Expires = DateTimeOffset.Now.AddDays(7),
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict
    };

    context.Response.Cookies.Append("account", jwtToken, options);
    context.Response.Redirect("/home");
});

// Run bot
if (Environment.GetEnvironmentVariable("PROFILE") == "RunDiscord")
{
    var bot = new DiscordBot(builder.Configuration["botToken"]!, app.Services);
    await bot.ConnectAsync();
}

// Resolve the BannerService from the DI container
var bannerService = app.Services.GetRequiredService<BannerService>();

// Call the SetBanner method
await bannerService.SetBanner();


app.Run();