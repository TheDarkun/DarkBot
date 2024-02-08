using DarkBot.Models;
using LiteDB;
using LiteDB.Async;

namespace DarkBot.Services;

public class PollService
{
    public async Task<PollModel> GetPollModel()
    {
        using var db = new LiteDatabaseAsync("Data.db");

        var pollCollection = db.GetCollection<PollModel>("Poll");

        var poll = await pollCollection.FindOneAsync(Query.All());

        if (poll is null)
        {
            var newPollModel = new PollModel(1, "", DateTime.Today.AddDays(-1), DateTime.Today, new List<OptionModel>());

            await pollCollection.InsertAsync(newPollModel);
            return newPollModel;
        }

        return poll;
    }

    public async Task AddNewPoll(PollModel poll)
    {
        using var db = new LiteDatabaseAsync("Data.db");
        var pollCollection = db.GetCollection<PollModel>("Poll");
        await pollCollection.InsertAsync(poll);
    }
}