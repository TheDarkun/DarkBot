using DarkBot.Services;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DarkBot.Discord.Commands;

public class QOTCommands(QOTService service) : ApplicationCommandModule
{
    private QOTService service { get; set; } = service;
    
    [SlashCommand("qoot", "Create question of the")]
    public async Task CreateQOT(InteractionContext ctx, [Option("question", "Put your question here")] string question)
    {
        await ctx.DeferAsync(true);

        var qot = await service.GetQOTModel();
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent($"Please select a channel on the website before creating QOT"));
        if (qot.ChannelId is null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent($"Please select a channel on the website before creating QOT"));
        }
        else if (qot.ChannelId == ctx.Channel.Id.ToString())
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent($"QOT is assigned to <#{qot.ChannelId}>"));
        }
        else if (qot.Date == DateTime.Today) // number.Date == DateTime.Today
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