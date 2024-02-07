using DarkBot.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkBot.Controllers;

public class AccountController(AccountService service) : Controller
{
    [HttpGet]
    public Task<IActionResult> Authorize()
        => Task.FromResult<IActionResult>(Redirect(service.GetRedirect()));

    [HttpGet]
    public async Task<IActionResult> Authenticate([FromQuery] string? code)
    {
        if (code is null)
            return Forbid();

        var account = await service.Authenticate(code);

        if (account is null)
            return Forbid();
        
        var user = await service.GetUser(account);
        
        if (user is null)
            return Forbid();

        account.UserId = user.Id;
        await service.SaveAccount(account);
        
        var jwtToken = service.CreateJwtToken(user);
        
        var options = new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddDays(7),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };
        
        Response.Cookies.Append("account", jwtToken.ToString(), options);

        return Redirect("/");
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("account");
        var id = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
        if (id is null)
            return Forbid();
        
        await service.DeleteAccount(id);
        return NoContent();
    }
}