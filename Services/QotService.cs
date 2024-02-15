using DarkBot.Clients;
using DarkBot.Models;
using LiteDB;

namespace DarkBot.Services;

public class QotService(DiscordHttpClient client)
{
    private DiscordHttpClient Client { get; } = client;
    
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
        var result = await Client.GetChannels(currentGuild.Id);
        return result;
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

            var result = await Client.GetCurrentChannelName(qot.ChannelId);
            return result;
        }
        catch (Exception)
        {
            return null;
        }
    }
}