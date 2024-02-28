using System.Text;
using DarkBot.Models.Banner;
using DarkBot.Services;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DarkBot.Discord.Commands;

public class BannerCommands(BannerService service) : ApplicationCommandModule
{
    [SlashCommand("add-banner", "Add a new banner!")]
    public async Task AddBanner(InteractionContext ctx,
        [Option("image", "Put your image here")]
        DiscordAttachment image)
    {
        await ctx.DeferAsync(true);
        var errors = new StringBuilder();
        if (image.MediaType is not ("image/jpeg" or "image/png"))
            errors.Append("- File has to be either .png or .jpeg \n");
        if (image.FileSize > 1024 * 1024 * 4)
            errors.Append("- File has to be less than 3 MB big \n");

        if (!string.IsNullOrWhiteSpace(errors.ToString()))
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent($"""
                              ## Could not upload image because:
                              {errors}
                              """));
            return;
        }

        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent($"Uploading image..."));
        var result = await service.SaveImageAttachment(image, ctx.User.Id);
        
        var message = result switch
        {
            BannerResult.Success => "Successfully added banner!",
            BannerResult.ImageLimit => "You already have **three** banners.",
            BannerResult.DatabaseError => "There was a database error.",
            BannerResult.Error => "Couldn't remove banner.",
            _ => "How?"
        };
        
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent(message));
    }

    [SlashCommand("show-banner", "Show your specific banner!")]
    public async Task ShowBanner(InteractionContext ctx,
        [Choice("First", "0")] [Choice("Second", "1")] [Choice("Third", "2")] [Option("Banner", "Which one?")]
        string option)
    {
        await ctx.DeferAsync(true);

        var banner = await service.GetBanner(ctx.User.Id, int.Parse(option));
        
        if (banner is null)
        {
            await ctx.EditResponseAsync(
                new DiscordWebhookBuilder(new DiscordMessageBuilder().WithContent("You don't have a banner here. Please add new one by using **/add-banner**")));
        }
        else
        {
            await ctx.EditResponseAsync(
                new DiscordWebhookBuilder(new DiscordMessageBuilder()
                    .AddFile(option + (banner.Contains(".png") ? ".png" : ".jpg"),
                        new MemoryStream(await File.ReadAllBytesAsync(
                            banner)))));
        }
        
        
    }

    [SlashCommand("delete-banner", "Delete banner you dislike!")]
    public async Task DeleteBanner(InteractionContext ctx,
        [Choice("First", "0")] [Choice("Second", "1")] [Choice("Third", "2")] [Option("Banner", "Which one?")]
        string option)
    {
        await ctx.DeferAsync(true);

        var result = await service.DeleteBanner(ctx.User.Id, int.Parse(option));

        var message = result switch
        {
            BannerResult.Success => "Successfully removed banner!",
            BannerResult.ImageLimit => "There is no banner here.",
            BannerResult.DatabaseError => "There was a database error.",
            BannerResult.Error => "Couldn't remove banner.",
            _ => "How?"
        };
        
        await ctx.EditResponseAsync(
            new DiscordWebhookBuilder(new DiscordMessageBuilder().WithContent(message)));
    }

    [SlashCommand("update-banner", "Replace your banner!")]
    public async Task UpdateBanner(InteractionContext ctx,
        [Choice("First", "0")] [Choice("Second", "1")] [Choice("Third", "2")] [Option("Banner", "Which one?")]
        string option,
        [Option("image", "Put your image here")]
        DiscordAttachment image)
    {
        await ctx.DeferAsync(true);

        var result = await service.UpdateBanner(ctx.User.Id, int.Parse(option), image);

        var message = result switch
        {
            BannerResult.Success => "Successfully updated banner!",
            BannerResult.ImageLimit => "There is no banner here.",
            BannerResult.DatabaseError => "There was a database error.",
            BannerResult.Error => "Couldn't remove banner.",
            _ => "How?"
        };
        
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(message));
    }
}