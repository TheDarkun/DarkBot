using DarkBot.Models;
using LiteDB;
using LiteDB.Async;

namespace DarkBot.Services;

public class QOTService
{
    public async Task<QOTModel> GetQOTModel()
    {
        using var db = new LiteDatabaseAsync("Data.db");
        // retrieve the entire QOT and its documents (each document is like one json object)
        var qotCollection = db.GetCollection<QOTModel>("QOT");
            
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
        using var db = new LiteDatabaseAsync(@"Data.db");
        var qotCollection = db.GetCollection<QOTModel>("QOT");
        await qotCollection.UpdateAsync(qot);
    }
}