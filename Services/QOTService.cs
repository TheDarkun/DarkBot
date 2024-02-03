using DarkBot.Models;
using LiteDB;

namespace DarkBot.Services;

public class QOTService
{
    public QOTModel GetQOTModel()
    {
        using LiteDatabase db = new LiteDatabase("Data.db");
        // retrieve the entire QOT and its documents (each document is like one json object)
        var qotCollection = db.GetCollection<QOTModel>("QOT");
            
        // select the first document from the array
        var qot = qotCollection.FindOne(Query.All());
            
        if (qot is null)
        {
            var newQOTModel = new QOTModel(1, DateTime.Today.AddDays(-1));
            // if for some reason the Data.db would have been deleted or corrupted, then new document will be created
            qotCollection.Insert(newQOTModel);
            return newQOTModel;
        }

        return qot;
    }
    
    public void UpdateQOTModel(QOTModel qot)
    {
        using LiteDatabase db = new LiteDatabase(@"Data.db");
        var qotCollection = db.GetCollection<QOTModel>("QOT");
        qotCollection.Update(qot);
    }
}