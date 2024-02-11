using DarkBot.Models;
using LiteDB;
using LiteDB.Async;

namespace DarkBot.Services;

public class HomeService(IConfiguration config, HttpClient client)
{
    private IConfiguration Config { get; } = config;
    private HttpClient Client { get; } = client;

    public async Task<ActiveGuildModel?> GetGuild()
    {
        using var db = new LiteDatabaseAsync("Data.db");

        var guildCollection = db.GetCollection<ActiveGuildModel>("Guild");
        var guild = await guildCollection.FindOneAsync(Query.All());

        return guild;
    }

    public async Task SetGuild(string id)
    {
        
    }
}