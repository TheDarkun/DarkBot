using DarkBot.Services;
using DSharpPlus.SlashCommands;

namespace DarkBot.Discord.Commands;

public class PollCommands(PollService service) : ApplicationCommandModule
{
    private PollService service { get; set; } = service;

    public async Task CreatePoll(InteractionContext ctx, [Option("question", "Put your question here")] string question, [Option("Ending time", "")]DateTime endDateTime)
    {
        await ctx.DeferAsync(true);

        var poll = await service.GetPollModel();
    }
}