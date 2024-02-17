using DarkBot.Discord.Commands;
using DarkBot.Models;
using DarkBot.Services;
using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace DarkBot.Discord;

public class DiscordBot(string token, IServiceProvider services)
{
    private BotService Service { get; set; } = new BotService();
    
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
        discord.GuildAvailable += async (s, e) =>
        {
            var guilds = new List<GuildModel>();
            foreach (var guild in discord.Guilds)
            {
                guilds.Add(new (guild.Value.Id.ToString(), guild.Value.Name));
            }
            await Service.CheckForNewGuilds(guilds);
        };

        discord.GuildDeleted += async (s, e) =>
        {
            await Service.RemoveGuild(e.Guild.Id.ToString());
        };
        
        discord.GuildCreated += async (s, e) =>
        {
            await Service.AddGuild(new (e.Guild.Id.ToString(), e.Guild.Name));
        };
        
        var slash = discord.UseSlashCommands(new SlashCommandsConfiguration
        {
            Services = services
        });
        slash.RegisterCommands<QotCommands>();
        await discord.ConnectAsync();
    }
}