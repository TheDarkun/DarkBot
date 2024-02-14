using LiteDB;

namespace DarkBot.Models;

public class ChannelModel(string id, string name)
{
    public string ChannelId { get; set; } = id;
    public string Name { get; set; } = name;
 
    public string Id { get; set; } = ObjectId.NewObjectId().ToString();
}