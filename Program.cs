using DarkBot.Controllers;
using DarkBot.Discord;
using DarkBot.Services;
using DarkBot.Web;
using Tailwind;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Transients
builder.Services.AddTransient<QOTService>();
builder.Services.AddTransient<QOTController>();

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
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller}/{action?}/{id?}"
);

// Create bot
var bot = new DiscordBot(builder.Configuration["token"]!, app.Services);
await bot.ConnectAsync();

app.Run();

