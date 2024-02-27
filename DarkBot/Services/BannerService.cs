using DSharpPlus.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace DarkBot.Services;

public class BannerService(IWebHostEnvironment env, HttpClient client)
{
    public async Task SaveBrowserImage(IBrowserFile image)
    {
        // TODO: GUID NAME
        var path = Path.Combine(env.WebRootPath, "images", $"{Path.GetRandomFileName()}{(image.ContentType == "image/png" ? ".png" : ".jpg")}");
        await using FileStream stream = new(path, FileMode.Create);
        
        var s = image.OpenReadStream(1024 * 1024 * 4);
        await s.CopyToAsync(stream);
    }

    public async Task<bool> SaveImageAttachment(DiscordAttachment image)
    {
        var result = await client.GetAsync(image.Url);
        if (result.IsSuccessStatusCode)
        {
            // TODO: GUID NAME
            var path = Path.Combine(env.WebRootPath, "images", $"{Path.GetRandomFileName()}{(image.MediaType == "image/png" ? ".png" : ".jpg")}");
            await using FileStream stream = new(path, FileMode.Create);
        
            var s = await result.Content.ReadAsStreamAsync();
            await s.CopyToAsync(stream);
            return true;
        }
        return false;
    }
}