using Newtonsoft.Json;

namespace DarkBot.Clients;

public class BackendHttpClient(HttpClient client)
{
    private HttpClient Client { get; set; } = client;
    
    public async Task PutAsJsonAsync(string path, object content)
        => await Client.PutAsJsonAsync(path, content);
    
    public async Task PostAsJsonAsync(string path, object content)
        => await Client.PostAsJsonAsync(path, content);

    public async Task<T?> GetAsync<T>(string path) where T : class?
    {
        try
        {
            var result = await Client.GetStringAsync(path);
            var content = JsonConvert.DeserializeObject<T>(result);
            return content;
        }
        catch (Exception)
        {
            return null;
        }
    }
}