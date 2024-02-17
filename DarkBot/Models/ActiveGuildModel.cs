using LiteDB;

namespace DarkBot.Models;

public class ActiveGuildModel
{
    public ActiveGuildModel(string id, string name, string? icon)
    {
        GuildId = id;
        Name = name;
        Icon = icon;
    }
    
    // this is necessary for the litedb to retrieve data, it manually sets each variable, for example ActiveGuildModel.Name = "name"
    public ActiveGuildModel() { }
    
    public string GuildId { get; set; }
    public string Name { get; set; }
    public string? Icon { get; set; }
    public string Id { get; set; } = ObjectId.NewObjectId().ToString();
}