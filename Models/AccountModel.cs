using LiteDB;

namespace DarkBot.Models;

public class AccountModel(string access_token, string refresh_token, int expires_in)
{
    public string AccessToken { get; set; } = access_token;
    public string RefreshToken { get; set; } = refresh_token;
    public DateTime ExpiresIn { get; set; } = DateTime.Now.AddSeconds(expires_in);
 
    public string Id { get; set; } = ObjectId.NewObjectId().ToString();
    
    public string? UserId { get; set; }
}