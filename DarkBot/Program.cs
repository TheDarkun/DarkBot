using System.Text;
using DarkBot.Clients;
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
builder.Services.AddHttpClient<BackendHttpClient>((provider, client) =>
{
    var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
    var token = httpContextAccessor.HttpContext!.Request.Cookies["account"];
    var baseUri = builder.Configuration["baseURI"]!;

    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    client.BaseAddress = new Uri(baseUri);
});
builder.Services.AddHttpClient<DiscordHttpClient>(client =>
{
    var token = builder.Configuration["botToken"]!;
    var apiVersion = builder.Configuration["apiVersion"]!;

    client.DefaultRequestHeaders.Add("Authorization", $"Bot {token}");
    client.BaseAddress = new Uri($"https://discord.com/api/v{apiVersion}");
});

// Transients
builder.Services.AddTransient<QotService>();

builder.Services.AddTransient<AccountService>();

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

    context.Response.Cookies.Append("account", jwtToken.ToString(), options);
    context.Response.Redirect("/Home");
});

// Run bot
if (Environment.GetEnvironmentVariable("PROFILE") == "RunDiscord")
{
    var bot = new DiscordBot(builder.Configuration["botToken"]!, app.Services);
    await bot.ConnectAsync();
}


app.Run();