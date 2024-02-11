using LiteDB;

namespace DarkBot.Models;

public class ActiveGuildModel(string id, string name, int? max_members, string? icon)
{
    public string GuildId { get; set; } = id;
    public string Name { get; set; } = name;
    public int? MaxMembers { get; set; } = max_members;
    public string? Icon { get; set; } = icon;
    
    public string Id { get; set; } = ObjectId.NewObjectId().ToString();
}