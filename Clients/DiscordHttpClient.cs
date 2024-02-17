﻿using DarkBot.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DarkBot.Clients;

public class DiscordHttpClient(IConfiguration config, HttpClient client)
{
    private IConfiguration Config { get; } = config;
    private HttpClient Client { get; } = client;

    public async Task<AccountModel?> Authenticate(string code)
    {
        try
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
            var response = await Client.PostAsync("oauth2/token", formContent);

            var content = await response.Content.ReadAsStringAsync();
            var account = JsonConvert.DeserializeObject<AccountModel>(content);
            return account;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<UserModel?> GetUser(AccountModel account)
    {
        try
        {
            Client.DefaultRequestHeaders.Remove("Authorization");
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {account.AccessToken}");
            var response =
                await Client.GetStringAsync("users/@me");
            var user = JsonConvert.DeserializeObject<UserModel>(response);
            return user;
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public async Task<ActiveGuildModel?> GetGuild(string id)
    {
        try
        {
            var result = await Client.GetStringAsync($"guilds/{id}");
            var guild = JsonConvert.DeserializeObject<ActiveGuildModel>(result);
            return guild;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public async Task<string?> GetCurrentChannelName(string id)
    {
        try
        {
            var result = await Client.GetStringAsync($"channels/{id}");
            var channel = JObject.Parse(result);
            return (string?)channel["name"];
        }
        catch (Exception)
        {
            return null;
        }
    }
    public async Task<List<ChannelModel>?> GetChannels(string id)
    {
        try
        {
            var result = await Client.GetStringAsync($"guilds/{id}/channels");
            var channels = JsonConvert.DeserializeObject<List<ChannelModel>>(result)!.Where(x => x.Type == "0").ToList();
            return channels;
        }
        catch (Exception)
        {
            return null;
        }
    }
}