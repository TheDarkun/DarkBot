using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DarkBot.Clients;
using DarkBot.Models;
using Microsoft.IdentityModel.Tokens;
using static System.Text.Encoding;

namespace DarkBot.Services;

public class AccountService(IConfiguration config, DiscordHttpClient client)
{
    private IConfiguration Config { get; } = config;
    private DiscordHttpClient Client { get; } = client;

    public string GetRedirect()
        => Config.GetSection("redirectURI").Value!;

    public async Task<AccountModel?> Authenticate(string code)
        => await Client.Authenticate(code);

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
        => await Client.GetUser(account);

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