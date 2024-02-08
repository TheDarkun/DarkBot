using System.ComponentModel.DataAnnotations;
using LiteDB;

namespace DarkBot.Models;

public class OptionModel(int index, string option, int count, string emojiName)
{
    [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
    public int Index { get; set; } = index;

    public string Option { get; set; } = option;

    public int Count { get; set; } = count;

    public string EmojiName { get; set; } = emojiName;
}