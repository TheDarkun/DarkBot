using DarkBot.Models;
using DarkBot.Models.Banner;
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

    public async Task<BannerResult> SaveImageAttachment(DiscordAttachment image, ulong userId)
    {
        try
        {
            var result = await client.GetAsync(image.Url);
            
            if (!result.IsSuccessStatusCode) return BannerResult.DatabaseError;

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
                return BannerResult.ImageLimit;
            }
            else
            {
                banner.Names.Add(name);
                await bannersCollection.UpdateAsync(banner);
            }

            return BannerResult.Success;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BannerResult.Error;
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

    public async Task<BannerResult> DeleteBanner(ulong userId, int parse)
    {
        try
        {
            var bannersCollection = Database.LiteDb.GetCollection<BannerModel>("Banner");
            if (bannersCollection is null)
                return BannerResult.DatabaseError;

            var banner = await bannersCollection.FindOneAsync(x => x.UserId == userId);
            banner.Names.RemoveAt(parse);
            await bannersCollection.UpdateAsync(banner);
            return BannerResult.Success;
        }
        catch (ArgumentOutOfRangeException e)
        {
            Console.WriteLine(e);
            return BannerResult.ImageLimit;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BannerResult.Error;
        }
    }

    public async Task<BannerResult> UpdateBanner(ulong userId, int parse, DiscordAttachment image)
    {
        try
        {
            var result = await DeleteBanner(userId, parse);
            return result == BannerResult.Success ? await SaveImageAttachment(image, userId) : result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BannerResult.Error;
        }
    }
}