using DarkBot.Models;
using DarkBot.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DarkBot.Controllers;

public class QOTController(QOTService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Data()
    {
        return Ok(await service.GetQOTModel());
    }

    [HttpPut]
    public async Task<IActionResult> Data([FromBody] QOTModel newQOT)
    {
        await service.UpdateQOTModel(newQOT);
        return Accepted();
    }

    [HttpGet]
    public async Task<IActionResult> Channels()
    {
        return Ok(new List<ChannelModel>()
        {
            new ChannelModel("10", "balls"),
            new ChannelModel("100", "balls"),
            new ChannelModel("10000", "balls"),
            new ChannelModel("1000", "balls")

        });
    }

    [HttpGet]
    public async Task<IActionResult> Channel()
    {
        return Ok("balls");
    }
    
    [HttpPost]
    public async Task<IActionResult> Channel([FromBody] string id)
    {
        return Accepted();
    }
    
    
}