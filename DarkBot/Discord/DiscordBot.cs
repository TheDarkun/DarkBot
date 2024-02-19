using DarkBot.Discord.Commands;
using DarkBot.Models;
using DarkBot.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LiteDB;

namespace DarkBot.Discord;

public class DiscordBot(string token, IServiceProvider services)
{
    private BotService BotService { get; set; } = services.GetService<BotService>()!;
    private QotService QotService { get; set; } = services.GetService<QotService>()!;

    
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
            await BotService.CheckForNewGuilds(guilds);
        };

        discord.GuildDeleted += async (s, e) =>
        {
            await BotService.RemoveGuild(e.Guild.Id.ToString());
        };
        
        discord.GuildCreated += async (s, e) =>
        {
            await BotService.AddGuild(new (e.Guild.Id.ToString(), e.Guild.Name));
        };

        
        discord.MessageCreated += async (s, e) =>
        {
            if (e.Author.IsBot)
                return;

            var id = await QotService.GetCurrentChannelId();

            if (e.Message.ChannelId.ToString() == id)
            {
                var replies = await QotService.GetReplies();
                if (replies.Any(user => user.Id == e.Author.Id.ToString()))
                {
                    var user = e.Author! as DiscordMember;
                    var dm = await user!.CreateDmChannelAsync();
                    await dm.SendMessageAsync("you are braindead");
                    await e.Message.DeleteAsync();
                    return;
                }

                await QotService.AddReply(e.Author as DiscordMember);
                return; // ALWAYS RETURN AFTER YOUR CONDITION IS FINISHED
            }
        };
        
        var slash = discord.UseSlashCommands(new SlashCommandsConfiguration
        {
            Services = services
        });
        slash.RegisterCommands<QotCommands>();
        await discord.ConnectAsync();
    }
}