using System.ComponentModel.DataAnnotations;
using LiteDB;

namespace DarkBot.Models;

public class QOTModel(int index, DateTime date)
{
    [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
    public int Index { get; set; } = index;

    public DateTime Date { get; set; } = date;
    
    public string? ChannelId { get; set; }
    public string _id { get; set; } = ObjectId.NewObjectId().ToString();
    public List<string> SubmittedUsers = new ();

    public void UpdateQOTModel()
    {
        Date = DateTime.Today;
        Index++;
    }
}