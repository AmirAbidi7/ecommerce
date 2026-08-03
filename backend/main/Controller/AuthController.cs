using main.dto.auth;
using main.Service;
using Microsoft.AspNetCore.Mvc;

namespace main.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResult>> Register([FromBody] RegisterUser user)
    {
        var authResult = await authService.RegisterAsync(user);
        SetRefreshToken(authResult.RefreshToken);
        return Ok(authResult);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResult>> Login([FromBody] LoginUser user)
    {
        var authResult = await authService.LoginAsync(user);
        SetRefreshToken(authResult.RefreshToken);
        return Ok(authResult);
    }

    [HttpGet("refresh")]
    public async Task<ActionResult<AuthResult>> Refresh()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var token))
        {
            return Unauthorized("Token not set");
        }
        var authResult = await authService.Refresh(token);
        SetRefreshToken(authResult.RefreshToken!);

        return Ok(authResult);
    }

    [HttpGet]
    public async Task<ActionResult> Logout()
    {
        Response.Cookies.Delete("refreshToken");
        return Ok();
    }

    private void SetRefreshToken(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/api/auth/refresh",
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
