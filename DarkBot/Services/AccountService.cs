using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DarkBot.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using static System.Text.Encoding;

namespace DarkBot.Services;

public class AccountService(IConfiguration config, HttpClient client)
{
    private IConfiguration Config { get; } = config;
    private HttpClient Client { get; } = client;

    public async Task<AccountModel?> Authenticate(string code)
    {
        var formData = new Dictionary<string, string>
        {
            { "client_id", Config.GetSection("clientId").Value! },
            { "client_secret", Config.GetSection("clientSecret").Value! },
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", $"{Config.GetSection("baseURI").Value!}/api/authorize" }
        };
        var formContent = new FormUrlEncodedContent(formData);
        var response = await Client.PostAsync($"oauth2/token", formContent);

        var content = await response.Content.ReadAsStringAsync();
        var account = JsonConvert.DeserializeObject<AccountModel>(content);
        return account;
    }

    public async Task SaveAccount(AccountModel account)
    {
        // TODO: Encrypt tokens

        var accountCollection = Database.LiteDb.GetCollection<AccountModel>("Accounts");

        await accountCollection.InsertAsync(account);
    }

    public async Task DeleteAccount(string id)
    {
        var accountCollection = Database.LiteDb.GetCollection<AccountModel>("Accounts");
        var accountExists = await accountCollection.FindOneAsync(x => x.UserId == id);

        if (accountExists is null)
            return;

        await accountCollection.DeleteAsync(accountExists.Id);
    }

    public async Task<UserModel?> GetUser(AccountModel account)
    {
        Client.DefaultRequestHeaders.Remove("Authorization");
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {account.AccessToken}");
        var response =
            await Client.GetStringAsync($"users/@me");
        var user = JsonConvert.DeserializeObject<UserModel>(response);
        return user;
    }

    public string CreateJwtToken(UserModel user)
    {
        List<Claim> claims = new List<Claim>
        {
            new("Username", user.Username ?? ""),
            new("GlobalName", user.GlobalName ?? ""),
            new("Id", user.Id ?? ""),
            new("Avatar", user.Avatar ?? ""),
            new("exp", ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString())
        };

        var key = new SymmetricSecurityKey(UTF8.GetBytes(Config.GetSection("randomJwtToken").Value!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var jwtToken = new JwtSecurityToken
        (
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds
        );

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(jwtToken);
    }
}