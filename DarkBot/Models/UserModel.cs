namespace DarkBot.Models;

public class UserModel(string id, string username, string global_name, string avatar)
{
    public string Id { get; set; } = id;
    public string Username { get; set; } = username;
    public string? GlobalName { get; set; } = global_name;
    public string? Avatar { get; set; } = avatar;
}