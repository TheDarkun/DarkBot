using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DarkBot.Discord.Commands;

public class QOTCommands : ApplicationCommandModule
{
    [SlashCommand("qot", "Create question of the")]
    public async Task CreateQOT(InteractionContext ctx, [Option("question", "Put your question here")] string question)
    {
        await ctx.DeferAsync();

        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent($"""
                          # QOT by {ctx.Member.Mention}
                          > {question}
                          """));
    }
}