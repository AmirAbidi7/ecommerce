using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AuthResult>> Register([FromBody] RegisterUser user)
    {
        var authResult = await authService.RegisterAsync(user);
        SetRefreshToken(authResult.RefreshToken);
        return Ok(authResult);
    }

    [HttpPost]
    public async Task<ActionResult<AuthResult>> Login([FromBody] LoginUser user)
    {
        var authResult = await authService.LoginAsync(user);
        SetRefreshToken(authResult.RefreshToken);
        return Ok(authResult);
    }

    [HttpGet]
    public async Task<ActionResult<AuthResult>> Refresh()
    {
        var token = Request.Cookies.First(cookie => cookie.Value == "refreshToken");
        var authResult = authService.Refresh(token.Value);

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
