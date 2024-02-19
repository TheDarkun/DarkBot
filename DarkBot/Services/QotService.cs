using DarkBot.Models;
using DSharpPlus.Entities;
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
            var newQotModel = new QotModel
            {
                Index = 1,
                Date = DateTime.Today.AddDays(-1)
            };
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
    
    public async Task<List<ChannelModel>?> GetChannels(ActiveGuildModel currentGuild)
    {
        var result = await Client.GetStringAsync($"guilds/{currentGuild.Id}/channels");
        var channels = JsonConvert.DeserializeObject<List<ChannelModel>>(result)!.Where(x => x.Type == "0").ToList();
        return channels;
    }

    public async Task<ActiveGuildModel?> GetCurrentGuild()
    {
        var currentGuildCollection = Database.LiteDb.GetCollection<ActiveGuildModel>("CurrentGuild");
        var currentGuild = await currentGuildCollection.FindOneAsync(Query.All());
        return currentGuild;
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
            var channelId = await GetCurrentChannelId();
            if (channelId is null)
                return null;
            
            var result = await Client.GetStringAsync($"channels/{channelId}");
            var channel = JObject.Parse(result);
            return (string?)channel["name"];
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public async Task<string?> GetCurrentChannelId()
    {
        try
        {
            var qot = await GetQotModel();
            if (qot.ChannelId is null)
                return null;
            return qot.ChannelId;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<UserModel>> GetReplies()
    {
        try
        {
            var qot = await GetQotModel();
            return qot.Replies;
        }
        catch (Exception)
        {
            return new ();
        }
    }

    public async Task AddReply(DiscordMember user)
    {
        try
        {
            var qot = await GetQotModel();
            qot.Replies.Add(new UserModel(user.Id.ToString(), user.Username, user.DisplayName, user.AvatarHash));
            await UpdateQotModel(qot);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task RemoveReply(string id)
    {
        try
        {
            var qot = await GetQotModel();
            qot.Replies.RemoveAll(user => user.Id == id);
            await UpdateQotModel(qot);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}