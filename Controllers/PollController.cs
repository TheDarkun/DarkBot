using DarkBot.Models;
using DarkBot.Services;
using Microsoft.AspNetCore.Mvc;

namespace DarkBot.Controllers;

public class PollController(PollService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Data()
    {
        return Ok(await service.GetPollModel());
    }

    [HttpPut]
    public async Task<IActionResult> Data([FromBody] PollModel poll)
    {
        await service.AddNewPoll(poll);
        return Accepted();
    }
}