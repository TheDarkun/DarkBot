using DarkBot.Models;
using DarkBot.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkBot.Controllers;

public class HomeController(HomeService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Guild()
    {
        var guild = await service.GetGuild();
        return Ok(guild);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Guild([FromBody] string id)
    {
        await service.SetGuild(id);
        return Accepted();
    }
}