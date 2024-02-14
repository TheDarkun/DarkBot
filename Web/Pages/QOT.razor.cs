using DarkBot.Clients;
using DarkBot.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DarkBot.Web.Pages;

public partial class QOT
{
    [Inject] public BackendHttpClient Client { get; set; } = null!;
    [Inject] public NavigationManager Manager { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    
    private QOTModel? qot { get; set; }

    private QOTModel? newQOT;

    protected override async Task OnInitializedAsync()
    {
        qot = await Client.GetAsync<QOTModel>("api/QOT/Data");
        newQOT = qot;
    }

    private async Task OnSetQOT()
    {
        var result = await JsRuntime.InvokeAsync<bool>("confirm", "Are you sure?");
        if (result)
            await Client.PutAsJsonAsync("api/QOT/Data", newQOT);
    }
}