using Microsoft.AspNetCore.Components.Forms;

namespace DarkBot.Services;

public class BannerService(IWebHostEnvironment env, HttpClient client)
{
    public async Task SaveFile(IBrowserFile file)
    {
        // TODO: GUID NAME
        var path = Path.Combine(env.WebRootPath, "images", "balls");
        await using FileStream stream = new(path, FileMode.Create);
        
        var s = file.OpenReadStream(1024 * 1024 * 4);
        await s.CopyToAsync(stream);
    }

    public async Task UploadFile(string url, bool isPng)
    {
        var result = await client.GetAsync(url);
        if (result.IsSuccessStatusCode)
        {
            // TODO: GUID NAME
            var path = Path.Combine(env.WebRootPath, "images", $"balls{(isPng ? ".png" : ".jpg")}");
            await using FileStream stream = new(path, FileMode.Create);
        
            var s = await result.Content.ReadAsStreamAsync();
            await s.CopyToAsync(stream); 
        }
    }
}