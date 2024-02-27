using DarkBot.Models;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace DarkBot.Services;

public class BannerService(IWebHostEnvironment env, HttpClient client)
{
    public async Task SaveBrowserImage(IBrowserFile image)
    {
        // TODO: GUID NAME
        var path = Path.Combine(env.WebRootPath, "images",
            $"{Path.GetRandomFileName()}{(image.ContentType == "image/png" ? ".png" : ".jpg")}");
        await using FileStream stream = new(path, FileMode.Create);

        var s = image.OpenReadStream(1024 * 1024 * 4);
        await s.CopyToAsync(stream);
    }

    public async Task<bool> SaveImageAttachment(DiscordAttachment image, ulong userId)
    {
        try
        {
            var result = await client.GetAsync(image.Url);
            
            if (!result.IsSuccessStatusCode) return false;

            var name = Path.GetRandomFileName() + (image.MediaType == "image/png" ? ".png" : ".jpg");


            var path = Path.Combine(env.WebRootPath, "images", name);
            await using FileStream stream = new(path, FileMode.Create);

            var s = await result.Content.ReadAsStreamAsync();
            await s.CopyToAsync(stream);

            var bannersCollection = Database.LiteDb.GetCollection<BannerModel>("Banner");
            var banner = await bannersCollection.FindOneAsync(x => x.UserId == userId);
            if (banner is null)
            {
                await bannersCollection.InsertAsync(new BannerModel(userId, new List<string> { name }));
            }
            else if (banner.Names.Count >= 3)
            {
                return false;
            }
            else
            {
                banner.Names.Add(name);
                await bannersCollection.UpdateAsync(banner);
            }

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<string?> GetBanner(ulong userId, int parse)
    {
        try
        {
            var bannersCollection = Database.LiteDb.GetCollection<BannerModel>("Banner");
            if (bannersCollection is null)
                return null;

            var banner = await bannersCollection.FindOneAsync(x => x.UserId == userId);
            var name = banner?.Names.ElementAtOrDefault(parse);
            return name is null ? null : Path.Combine(env.WebRootPath, "images", name);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}