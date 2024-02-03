using DarkBot.Discord.Commands;
using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace DarkBot.Discord;

public class DiscordBot(string token, IServiceProvider services)
{
    private DiscordClient discord { get; set; } = new(new()
    {
        Token = token,
        TokenType = TokenType.Bot,
        Intents = DiscordIntents.AllUnprivileged,
        AutoReconnect = true
    });

    private IServiceProvider services { get; set; } = services;
    
    public async Task ConnectAsync()
    {
        var slash = discord.UseSlashCommands(new SlashCommandsConfiguration
        {
            Services = services
        });
        slash.RegisterCommands<QOTCommands>();
        await discord.ConnectAsync();
    }
}