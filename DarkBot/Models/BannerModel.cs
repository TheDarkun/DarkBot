using LiteDB;

namespace DarkBot.Models;

public class BannerModel
{
    public string Id { get; set; } = ObjectId.NewObjectId().ToString();
    
    public ulong UserId { get; set; }
    public List<string> Names { get; set; } = null!;

    public BannerModel(ulong userId, List<string>? names)
    {
        UserId = userId;
        Names = names ?? new();
    }
    
    public BannerModel()
    {
    }
}