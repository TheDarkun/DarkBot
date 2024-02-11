using LiteDB;

namespace DarkBot.Models;

public class GuildModel(string id, string name)
{
    public string GuildId { get; set; } = id;
    public string Name { get; set; } = name;
 
    public string Id { get; set; } = ObjectId.NewObjectId().ToString();
}