using DarkBot.Clients;
using DarkBot.Models;
using DarkBot.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DarkBot.Web.Pages;

public partial class QotPage
{
    [Inject] public QotService Service { get; set; } = null!;
    [Inject] public NavigationManager Manager { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    
    private QotModel? Qot { get; set; }

    private QotModel? newQot;

    private string? ActiveChannel { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        Qot = await Service.GetQotModel();
        newQot = Qot;
        ActiveChannel = await Service.GetCurrentChannelName();
    } 

    private async Task OnSetQOT()
    {
        var result = await JsRuntime.InvokeAsync<bool>("confirm", "Are you sure?");
        if (result)
            await Service.UpdateQotModel(newQot);
    }
}