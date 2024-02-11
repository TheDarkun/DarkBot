using DarkBot.Models;
using LiteDB;
using Newtonsoft.Json;

namespace DarkBot.Services;

public class HomeService(IConfiguration config, HttpClient client)
{
    private IConfiguration Config { get; } = config;
    private HttpClient Client { get; } = client;
    public async Task<ActiveGuildModel?> GetCurrentGuild()
    {
        var guildCollection = Database.LiteDb.GetCollection<ActiveGuildModel>("CurrentGuild");
        var guild = await guildCollection.FindOneAsync(Query.All());

        return guild;
    }

    public async Task<List<GuildModel>?> GetJoinedGuilds()
    {
        // using var db = new LiteDatabaseAsync("Data.db");
        var guildCollection = Database.LiteDb.GetCollection<GuildModel>("Guild");
        var storedGuilds = await guildCollection.FindAllAsync();
        return storedGuilds.ToList();
    }
    public async Task SetGuild(string id)
    {
        // using var db = new LiteDatabaseAsync("Data.db");
        var guildCollection = Database.LiteDb.GetCollection<ActiveGuildModel>("CurrentGuild");
        await guildCollection.DeleteAllAsync();

        client.DefaultRequestHeaders.Add("Authorization", "Bot MTIwNDA3Nzg4OTc1MDk2NjMzMg.GSDsRk.RRe0Cb8GlW13e7oLswvnnNuKLhYparzJVUDbRo");
        var result = await client.GetAsync($"https://discordapp.com/api/guilds/{id}");
        if (result.IsSuccessStatusCode)
        {
            var content = await result.Content.ReadAsStringAsync();
            var guild = JsonConvert.DeserializeObject<ActiveGuildModel>(content);
            await guildCollection.InsertAsync(guild);
        }
    }
}