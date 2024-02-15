using DarkBot.Models;
using DarkBot.Services;
using Microsoft.AspNetCore.Mvc;

namespace DarkBot.Controllers;

public class QotController(QotService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Data()
    {
        return Ok(await service.GetQotModel());
    }

    [HttpPut]
    public async Task<IActionResult> Data([FromBody] QotModel newQot)
    {
        await service.UpdateQotModel(newQot);
        return Accepted();
    }

    [HttpGet]
    public async Task<IActionResult> Channels()
    {
        var channels = await service.GetChannels();
        if (channels is null)
            return new StatusCodeResult(406);
        return Ok(channels);
    }

    [HttpGet]
    public async Task<IActionResult> Channel()
    {
        var channel = await service.GetCurrentChannelName();
        if (channel is null)
            return new StatusCodeResult(406);
        return Ok(channel);
    }
    
    [HttpPost]
    public async Task<IActionResult> Channel([FromBody] string id)
    {
        await service.UpdateChannel(id);
        return Accepted();
    }
    
    
}