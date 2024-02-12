using DarkBot.Models;
using DarkBot.Services;
using Microsoft.AspNetCore.Authorization;
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
    // [Authorize]
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
        var name = await service.GetCurrentChannel();
        if (name is null)
            return new StatusCodeResult(406); 
        
        return Ok(name);
    }
    
    [HttpPost]
    public async Task<IActionResult> Channel([FromBody] string id)
    {
        Console.WriteLine(id);
        await service.UpdateChannel(id);
        
        return Accepted();
    }
    
    
}