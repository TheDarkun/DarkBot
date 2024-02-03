using DarkBot.Models;
using DarkBot.Services;
using Microsoft.AspNetCore.Mvc;

namespace DarkBot.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QOTController(QOTService service) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(service.GetQOTModel());
    }

    [HttpPut]
    public IActionResult Put([FromBody] QOTModel newQOT)
    {
        service.UpdateQOTModel(newQOT);
        return Accepted();
    }
}