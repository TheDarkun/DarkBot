using DarkBot.Models;
using LiteDB;
using Newtonsoft.Json;

namespace DarkBot.Services;

public class HomeService(HttpClient client)
{
    private HttpClient Client { get; } = client;
    public async Task<ActiveGuildModel?> GetCurrentGuild()
    {
        var guildCollection = Database.LiteDb.GetCollection<ActiveGuildModel>("CurrentGuild");
        var guild = await guildCollection.FindOneAsync(Query.All());

        return guild;
    }

    public async Task<List<GuildModel>?> GetJoinedGuilds()
    {
        var guildCollection = Database.LiteDb.GetCollection<GuildModel>("Guild");
        var storedGuilds = await guildCollection.FindAllAsync();
        return storedGuilds.ToList();
    }
    public async Task SetGuild(string id)
    {
        var guildCollection = Database.LiteDb.GetCollection<ActiveGuildModel>("CurrentGuild");
        await guildCollection.DeleteAllAsync();
        var result = await Client.GetStringAsync($"guilds/{id}");
        var guild = JsonConvert.DeserializeObject<ActiveGuildModel>(result);
        
        if (guild is not null)
            await guildCollection.InsertAsync(guild);
        
    }
}