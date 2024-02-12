using DarkBot.Models;
using DarkBot.Web.Components.Home;
using LiteDB;
using LiteDB.Async;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DarkBot.Services;

public class QOTService(IConfiguration config, HttpClient client)
{
    private IConfiguration Config { get; set; } = config;
    private HttpClient Client { get; set; } = client;

    public async Task<QOTModel> GetQOTModel()
    {
        var qotCollection = Database.LiteDb.GetCollection<QOTModel>("QOT");

        // select the first document from the array
        var qot = await qotCollection.FindOneAsync(Query.All());

        if (qot is null)
        {
            var newQOTModel = new QOTModel(1, DateTime.Today.AddDays(-1));
            // if for some reason the Data.db would have been deleted or corrupted, then new document will be created
            await qotCollection.InsertAsync(newQOTModel);
            return newQOTModel;
        }

        return qot;
    }

    public async Task UpdateQOTModel(QOTModel qot)
    {
        var qotCollection = Database.LiteDb.GetCollection<QOTModel>("QOT");
        await qotCollection.UpdateAsync(qot);
    }

    public async Task<List<ChannelModel>?> GetChannels()
    {
        Client.DefaultRequestHeaders.Add("Authorization", $"Bot {Config.GetSection("botToken").Value!}");
        var currentGuildCollection = Database.LiteDb.GetCollection<ActiveGuildModel>("CurrentGuild");
        var currentGuild = await currentGuildCollection.FindOneAsync(Query.All());
        var result = await Client.GetStringAsync($"https://discordapp.com/api/guilds/{currentGuild.Id}/channels");
        var a = JsonConvert.DeserializeObject<List<ChannelModel>>(result).Where(x => x.Type == "0").ToList();
        return a;
    }

    public async Task UpdateChannel(string id)
    {
        var qotCollection = Database.LiteDb.GetCollection<QOTModel>("QOT");
        var qot = await qotCollection.FindOneAsync(Query.All());
        qot.ChannelId = id;
        await qotCollection.UpdateAsync(qot);
    }

    public async Task<string?> GetCurrentChannel()
    {
        try
        {
            var qotCollection = Database.LiteDb.GetCollection<QOTModel>("QOT");
            var qot = await qotCollection.FindOneAsync(Query.All());
            if (qot.ChannelId is null)
                return null;

            Client.DefaultRequestHeaders.Add("Authorization", $"Bot {Config.GetSection("botToken").Value!}");
            var result = await Client.GetStringAsync($"https://discordapp.com/api/v9/channels/{qot.ChannelId}");

            var obj = JObject.Parse(result);
            return (string)obj["name"]!;
        }
        catch (Exception)
        {
            return null;
        }
    }
}