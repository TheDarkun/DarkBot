using DarkBot.Models;
using DarkBot.Services;
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
}