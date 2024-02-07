﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DarkBot.Models;
using LiteDB.Async;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using static System.Text.Encoding;

namespace DarkBot.Services;

public class AccountService(IConfiguration config, HttpClient client)
{
    private IConfiguration Config { get; } = config;
    private HttpClient Client { get; } = client;

    public string GetRedirect()
        => Config.GetSection("redirectURI").Value!;
    
    public async Task<AccountModel?> Authenticate(string code)
    {
        var formData = new Dictionary<string, string>
        {
            { "client_id", Config.GetSection("clientId").Value! },
            { "client_secret", Config.GetSection("clientSecret").Value! },
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", $"{Config.GetSection("baseURI").Value!}api/Account/Authenticate" }
        };

        var formContent = new FormUrlEncodedContent(formData);
        Client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
        var response =
            await Client.PostAsync(
                $"https://discord.com/api/v{Config.GetSection("apiVersion").Value!}/oauth2/token",
                formContent);
        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        var account = JsonConvert.DeserializeObject<AccountModel>(content);

        return account;
    }
    
    public async Task SaveAccount(AccountModel account)
    {
        // TODO: Encrypt tokens
        
        using var db = new LiteDatabaseAsync("Data.db");

        var accountCollection = db.GetCollection<AccountModel>("Accounts");

        await accountCollection.InsertAsync(account);
    }

    public async Task<UserModel?> GetUser(AccountModel account)
    {
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {account.AccessToken}");
        var response =
            await Client.GetAsync($"https://discord.com/api/v{Config.GetSection("apiVersion").Value!}/users/@me");
        
        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        var user = JsonConvert.DeserializeObject<UserModel>(content);

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