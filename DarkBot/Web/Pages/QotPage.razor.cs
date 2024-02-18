using DarkBot.Models;
using DarkBot.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DarkBot.Web.Pages;

public partial class QotPage
{
    [Inject] public NavigationManager Manager { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    
    

    
    //
    //
    // private async Task OnSetQOT()
    // {
    //     var result = await JsRuntime.InvokeAsync<bool>("confirm", "Are you sure?");
    //     if (result)
    //         await Service.UpdateQotModel(newQot);
    // }
}