using DarkBot.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkBot.Controllers;

public class HomeController(HomeService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> CurrentGuild()
    {
        var guild = await service.GetCurrentGuild();
        if (guild is null)
            return new StatusCodeResult(406); // This response is sent when the web server, after performing server-driven content negotiation, doesn't find any content that conforms to the criteria given by the user agent.
        return Ok(guild);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CurrentGuild([FromBody] string id)
    {
        await service.SetGuild(id);
        return Accepted();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetJoinedGuilds()
    {
        var guilds = await service.GetJoinedGuilds();
        if (guilds is null)
            return new StatusCodeResult(406);
        return Ok(guilds);
    }
}