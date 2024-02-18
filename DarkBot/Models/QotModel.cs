using System.ComponentModel.DataAnnotations;
using LiteDB;

namespace DarkBot.Models;

public class QotModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
    public int Index { get; set; }
    
    public DateTime Date { get; set; }
    
    [Required(ErrorMessage = "Please select a channel!")]
    public string? ChannelId { get; set; }
    
    public string Id { get; set; } = ObjectId.NewObjectId().ToString();
    // userId = userName
    public List<UserModel> Replies { get; set; } = new();
    
    public void UpdateQotModel()
    {
        Date = DateTime.Today;
        Index++;
        Replies.Clear();
    }
}