using System.Text;
using DarkBot.Models;
using DarkBot.Models.Banner;
using DSharpPlus.Entities;
using LiteDB;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;

namespace DarkBot.Services;

public class BannerService(IWebHostEnvironment env, HttpClient client, IServiceProvider provider)
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

    public async Task<BannerResult> SaveImageAttachment(DiscordAttachment image, ulong userId, int parse = -1)
    {
        try
        {
            var result = await client.GetAsync(image.Url);
            
            if (!result.IsSuccessStatusCode) return BannerResult.DatabaseError;

            var name = Path.GetRandomFileName() + (image.MediaType == "image/png" ? ".png" : ".jpg");

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
                if (parse == -1)
                {
                    banner.Names.Add(name);
                }
                else
                {
                    banner.Names.Insert(parse, name);
                }
                
                await bannersCollection.UpdateAsync(banner);
            }

            var path = Path.Combine(env.WebRootPath, "images", name);
            await using FileStream stream = new(path, FileMode.Create);

            var s = await result.Content.ReadAsStreamAsync();
            await s.CopyToAsync(stream);
            
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

            var name = banner.Names.ElementAtOrDefault(parse);
            
            banner.Names.RemoveAt(parse);
            if (name is not null)
            {
                var e = $@"{env.WebRootPath}\images\{name}";
                Console.WriteLine(e);
                File.Delete(e);
            }
            
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
            return result == BannerResult.Success ? await SaveImageAttachment(image, userId, parse) : result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BannerResult.Error;
        }
    }

    public async Task SetBanner()
    {
        try
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<BannerChangeJob>()
                .WithIdentity("myJob", "group1")
                .UsingJobData(new JobDataMap {{"provider", provider}})
                .Build();

            // ITrigger trigger = TriggerBuilder.Create()
            //     .WithIdentity("myTrigger", "group1")
            //     .StartNow()
            //     .
            //     .WithCronSchedule("0 32 19 ? * * *") // Fires every day at 8pm. //                 .WithCronSchedule("0 0 20 ? * * *") // Fires every day at 8pm.1
            //     .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever())
                .Build();
            
            await scheduler.ScheduleJob(job, trigger);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
public class BannerChangeJob : IJob
{
    private Random random = new Random(Guid.NewGuid().GetHashCode());
    
    public async Task Execute(IJobExecutionContext context)
    {
        var provider = (IServiceProvider)context.JobDetail.JobDataMap.Get("provider");
        using var scope = provider.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<HttpClient>();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            
        var guild = await Database.LiteDb.GetCollection<ActiveGuildModel>("CurrentGuild").FindOneAsync(Query.All());
        if (guild is null)
        {
            Console.WriteLine("Guild is null, couldn't update server banner");
            return;
        }

        var banner = await SelectRandomBanner();
        if (banner is null) return;
            
        var path = Path.Combine(env.WebRootPath, "images", banner);
            
        byte[] imageBytes = await File.ReadAllBytesAsync(path);
        string base64Image = Convert.ToBase64String(imageBytes);

        var aaaa = $"data:image/{(banner.Contains(".png") ? "png" : "jpeg")};base64,{base64Image}";

        var content = new StringContent("{\"banner\": \"" + aaaa + "\"}", Encoding.UTF8, "application/json");
        var result = await client.PatchAsync($"guilds/{guild.Id}", content);
        Console.WriteLine(client.DefaultRequestHeaders.ToString());
        Console.WriteLine(client.BaseAddress + $"/guilds/{guild.Id}");

        Console.WriteLine("yep");
        var eeeeee = await result.Content.ReadAsStringAsync();
        Console.WriteLine(eeeeee);
    }

    private async Task<string?> SelectRandomBanner()
    {
        try
        {
            var bannersCollection = Database.LiteDb.GetCollection<BannerModel>("Banner");
            if (bannersCollection is null)
                return null;

            var banners = (await bannersCollection.FindAsync(Query.All()))?.ToArray();

            BannerModel? user = null;
            BannerModel? cooldownUser = banners?.FirstOrDefault(x => x.HasCooldown);

            for (int i = 0; i < 10; i++)
            {
                user = banners?[random.Next(0, banners.Length)];
                if (user is not { HasCooldown: false }) continue;
                
                if (cooldownUser != null) cooldownUser.HasCooldown = false;
                user.HasCooldown = true;

                if (cooldownUser != null) await bannersCollection.UpdateAsync(cooldownUser);
                await bannersCollection.UpdateAsync(user);

                break;
            }
            
            var banner = user?.Names[random.Next(0, user.Names.Count)];
            Console.WriteLine($"user {user?.UserId} | banner {banner}");
            return banner;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}
