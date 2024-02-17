using DarkBot.Models;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DarkBot.Services;

public class QotService(HttpClient client)
{
    private HttpClient Client { get; } = client;
    
    public async Task<QotModel> GetQotModel()
    {
        var qotCollection = Database.LiteDb.GetCollection<QotModel>("QOT");
            
        // select the first document from the array
        var qot = await qotCollection.FindOneAsync(Query.All());
            
        if (qot is null)
        {
            var newQotModel = new QotModel(1, DateTime.Today.AddDays(-1));
            // if for some reason the Data.db would have been deleted or corrupted, then new document will be created
            await qotCollection.InsertAsync(newQotModel);
            return newQotModel;
        }

        return qot;
    }
    
    public async Task UpdateQotModel(QotModel qot)
    {
        var qotCollection = Database.LiteDb.GetCollection<QotModel>("QOT");
        await qotCollection.UpdateAsync(qot);
    }
    
    public async Task<List<ChannelModel>?> GetChannels()
    {
        var currentGuildCollection = Database.LiteDb.GetCollection<ActiveGuildModel>("CurrentGuild");
        var currentGuild = await currentGuildCollection.FindOneAsync(Query.All());
        var result = await Client.GetStringAsync($"guilds/{currentGuild.Id}/channels");
        var channels = JsonConvert.DeserializeObject<List<ChannelModel>>(result)!.Where(x => x.Type == "0").ToList();
        return channels;
    }

    public async Task UpdateChannel(string id)
    {
        var qotCollection = Database.LiteDb.GetCollection<QotModel>("QOT");
        var qot = await qotCollection.FindOneAsync(Query.All());
        qot.ChannelId = id;
        await qotCollection.UpdateAsync(qot);
    }

    public async Task<string?> GetCurrentChannelName()
    {
        try
        {
            var qotCollection = Database.LiteDb.GetCollection<QotModel>("QOT");
            var qot = await qotCollection.FindOneAsync(Query.All());
            if (qot.ChannelId is null)
                return null;

            var result = await Client.GetStringAsync($"channels/{qot.ChannelId}");
            var channel = JObject.Parse(result);
            return (string?)channel["name"];
        }
        catch (Exception)
        {
            return null;
        }
    }
}