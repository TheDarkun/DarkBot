using LiteDB;

namespace DarkBot.Models;

public class ChannelModel(string id, string name, string type)
{
    public string ChannelId { get; set; } = id;
    public string Name { get; set; } = name;
    public string Type { get; set; } = type;
    
    public string Id { get; set; } = ObjectId.NewObjectId().ToString();
}