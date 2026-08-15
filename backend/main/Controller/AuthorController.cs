using System.Security.Claims;
using main.DTO.author;
using main.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace main.Controller;

[ApiController]
[Route("/api/[controller]")]
[Authorize(Roles = "Author")]
public class AuthorController(AuthorService authorService) : ControllerBase
{
    [HttpGet("products")]
    public async Task<ActionResult<ICollection<AuthorBook>>> GetMyProducts()
    {
        var userId = new Guid(User.FindFirstValue("UserId")!);
        return Ok(await authorService.GetMyBooksAsync(userId));
    }

    [HttpGet("sales")]
    public async Task<ActionResult<ICollection<AuthorSaleStat>>> GetSales()
    {
        var userId = new Guid(User.FindFirstValue("UserId")!);
        return Ok(await authorService.GetSalesAsync(userId));
    }
}