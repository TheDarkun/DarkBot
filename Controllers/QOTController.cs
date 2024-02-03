using DarkBot.Models;
using DarkBot.Services;
using Microsoft.AspNetCore.Mvc;

namespace DarkBot.Controllers;

public class QOTController(QOTService service) : ControllerBase
{
    [HttpGet]
    public IActionResult Data()
    {
        return Ok(service.GetQOTModel());
    }

    [HttpPut]
    public IActionResult Data([FromBody] QOTModel newQOT)
    {
        service.UpdateQOTModel(newQOT);
        return Accepted();
    }
}