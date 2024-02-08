using System.ComponentModel.DataAnnotations;
using LiteDB;

namespace DarkBot.Models;

public class PollModel(int index, string topic, DateTime startDateTime, DateTime endDateTime, List<OptionModel> options)
{
    [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
    public int Index { get; set; } = index;

    public string Topic { get; set; } = topic;

    public DateTime StartDateTime { get; set; } = startDateTime;
    public DateTime EndDateTime { get; set; } = endDateTime;

    public List<OptionModel> Options { get; set; } = options;
    
    public string Id { get; set; } = ObjectId.NewObjectId().ToString();
}