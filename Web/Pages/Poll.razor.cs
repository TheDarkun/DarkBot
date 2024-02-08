using DarkBot.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DarkBot.Web.Pages;

public partial class Poll
{
    [Inject] public HttpClient Client { get; set; } = null!;
    [Inject] public NavigationManager Manager { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;

    private PollModel? poll;
    
    private PollModel newPoll = new (1, "", DateTime.Now, DateTime.Now.AddDays(1), new List<OptionModel>());
    
    
    private int counter;

    protected override async Task OnInitializedAsync()
    {
        int index = 1;
        
        // var result = await Client.GetAsync($"{Manager.BaseUri}api/poll/Data");
        // if (result.IsSuccessStatusCode)
        // {
        //     var content = await result.Content.ReadAsStringAsync();
        //     poll = JsonConvert.DeserializeObject<PollModel>(content);
        //     index = poll!.Index + 1;
        // }

        newPoll.Index = index;
        newPoll.Options.Add(new OptionModel(1, "", 0, ""));
        counter = 0;
    }


    private void AddOption()
    {
        counter++;
        newPoll!.Options.Add(new OptionModel(counter + 1, "", 0, ""));
    }

    private void RemoveOption(OptionModel option)
    {
        newPoll!.Options.Remove(option);
    }


    private async Task OnSetPoll()
    {
        var result = await JsRuntime.InvokeAsync<bool>("confirm", "Are you sure?");
        if (result)
            await Client.PutAsJsonAsync($"{Manager.BaseUri}api/Poll/Data", newPoll);
    }
}