using DarkBot.Services;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DarkBot.Discord.Commands;

public class QOTCommands(QOTService service) : ApplicationCommandModule
{
    private QOTService service { get; set; } = service;
    
    [SlashCommand("qot", "Create question of the")]
    public async Task CreateQOT(InteractionContext ctx, [Option("question", "Put your question here")] string question)
    {
        await ctx.DeferAsync(true);

        var qot = await service.GetQOTModel();

        if (qot.Date == DateTime.Today) // number.Date == DateTime.Today
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent("There is already a question today!"));
        }
        else
        {
            qot.UpdateQOTModel();
            await service.UpdateQOTModel(qot);
            
            await ctx.DeleteResponseAsync();
            
            // Send a follow-up message instead of editing the response.
            await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder()
                .WithContent($"""
                              # QOT #{qot.Index} by {ctx.Member.Mention}
                              > {question}
                              """));
        }
    }
}