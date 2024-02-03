using DarkBot.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace DarkBot.Web.Pages;

public partial class QOT
{
    [Inject] public HttpClient Client { get; set; } = null!;
    [Inject] public NavigationManager Manager { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    
    private QOTModel? qot { get; set; }

    private QOTModel? newQOT;

    protected override async Task OnInitializedAsync()
    {
        var result = await Client.GetAsync($"{Manager.BaseUri}api/QOT/Data");
        Console.WriteLine(result.IsSuccessStatusCode);
        if (result.IsSuccessStatusCode)
        {
            var content = await result.Content.ReadAsStringAsync();
            qot = JsonConvert.DeserializeObject<QOTModel>(content);
            newQOT = qot;
        }

    }

    private async Task OnSetQOT()
    {
        var result = await JsRuntime.InvokeAsync<bool>("confirm", "Are you sure?");
        if (result)
            await Client.PutAsJsonAsync($"{Manager.BaseUri}api/QOT/Data", newQOT);
    }
}