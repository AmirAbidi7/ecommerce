using System.Security.Claims;
using main.DTO.product;
using main.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace main.Controller;

[ApiController]
[Route("/api/[controller]")]
public class ProductController(ProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ICollection<ProductOverview>>> GetProducts()
    {
        var productList = await productService.GetProductsAsync();

        return Ok(productList);
    }

    [HttpGet("{productId:guid}")]
    public async Task<ActionResult<ProdutDetails>> GetProduct([FromRoute] Guid productId)
    {
        var product = await productService.GetProductAsync(productId);

        return Ok(product);
    }

    [HttpPut("favorite")]
    [Authorize]
    public async Task<ActionResult> FavoriteProduct([FromBody] Guid productId)
    {
        var userId = User.FindFirstValue("UserId");
        await productService.FavoriteProduct(productId, new Guid(userId!));
        return Ok();
    }

    [HttpPut("unfavorite")]
    [Authorize]
    public async Task<ActionResult> UnfavoriteProduct([FromBody] Guid productId)
    {
        var userId = User.FindFirstValue("UserId");
        await productService.UnfavoriteProduct(productId, new Guid(userId!));
        return Ok();
    }

    [HttpPost]
    [Authorize(Roles = "Author")]
    public async Task<ActionResult<ProductOverview>> CreateProduct([FromBody] CreateProductRequest request)
    {
        var userId = new Guid(User.FindFirstValue("UserId")!);
        var product = await productService.CreateProductAsync(request, userId);
        return Ok(product);
    }

    [HttpDelete("{productId:guid}")]
    [Authorize(Roles = "Author")]
    public async Task<ActionResult> UnlistProduct([FromRoute] Guid productId)
    {
        var userId = new Guid(User.FindFirstValue("UserId")!);
        await productService.UnlistProductAsync(productId, userId);
        return Ok();
    }

    [HttpGet("favorites")]
    [Authorize]
    public async Task<ActionResult<ICollection<Guid>>> GetFavoriteIds()
    {
        var userId = new Guid(User.FindFirstValue("UserId")!);
        return Ok(await productService.GetFavoriteIdsAsync(userId));
    }
}
