using DarkBot.Models;
using LiteDB.Async;

namespace DarkBot.Services;

public class BotService
{
    public async Task CheckForNewGuilds(List<GuildModel> guilds)
    {
        var guildCollection = Database.LiteDb.GetCollection<GuildModel>("Guild");
        var storedGuilds = await guildCollection.FindAllAsync();

        var addedGuilds = new List<GuildModel>();

        var guildModels = storedGuilds.ToList();
        foreach (var guild in guilds)
        {
            if (!guildModels.Contains(guild))
            {
                addedGuilds.Add(guild);
            }
        }

        foreach (var newGuild in addedGuilds)
        {
            await guildCollection.InsertAsync(newGuild);
            guildModels.Add(newGuild);
        }
        
        var removedGuilds = new List<GuildModel>();

        foreach (var storedGuild in guildModels)
        {
            if (!guilds.Contains(storedGuild))
            {
                removedGuilds.Add(storedGuild);
            }
        }

        foreach (var removedGuild in removedGuilds)
        {
            await guildCollection.DeleteAsync(removedGuild.Id);
        }
        
    }

    public async Task RemoveGuild(string id)
    {
        // using var db = new LiteDatabaseAsync("Data.db");
        var guildCollection = Database.LiteDb.GetCollection<GuildModel>("Guild");
        var guild = await guildCollection.FindOneAsync(x => x.GuildId == id);
        await guildCollection.DeleteAsync(guild.Id);
    }

    public async Task AddGuild(GuildModel guild)
    {
        // using var db = new LiteDatabaseAsync("Data.db");
        var guildCollection = Database.LiteDb.GetCollection<GuildModel>("Guild");
        await guildCollection.InsertAsync(guild);
    }
}