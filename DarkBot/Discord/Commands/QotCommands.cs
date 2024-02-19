using DarkBot.Services;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DarkBot.Discord.Commands;

public class QotCommands(QotService service) : ApplicationCommandModule
{
    private QotService Service { get; } = service;
    
    [SlashCommand("qot", "Create question of the")]
    public async Task CreateQot(InteractionContext ctx, [Option("question", "Put your question here")] string question)
    {
        await ctx.DeferAsync(true);

        var qot = await Service.GetQotModel();

        if (qot.ChannelId is null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent($"Please select a channel on the website before creating QOT"));
        }
        else if (qot.ChannelId != ctx.Channel.Id.ToString())
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
            qot.UpdateQotModel();
            await Service.UpdateQotModel(qot);
             
            await ctx.DeleteResponseAsync();
             
            // Send a follow-up message instead of editing the response.
            await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder()
                .WithContent($"""
                              # QOT #{qot.Index-1} by {ctx.Member.Mention}
                              > {question}
                              """));
        }
    }
}