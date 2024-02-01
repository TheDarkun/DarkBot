using DSharpPlus;

namespace DarkBot.Discord;

public class DiscordBot(string token)
{
    private DiscordClient discord { get; set; } = new(new()
    {
        Token = token,
        TokenType = TokenType.Bot,
        Intents = DiscordIntents.AllUnprivileged,
        AutoReconnect = true
    });

    public async Task ConnectAsync()
    {
        await discord.ConnectAsync();
        await Task.Delay(-1);
    }
}