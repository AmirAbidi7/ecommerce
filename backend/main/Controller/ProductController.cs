using System.Security.Claims;
using main.DTO.product;
using main.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace main.Controller;

[ApiController]
[Route("/api/[controller]")]
public class ProductController(ProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ICollection<ProductList>>> GetProducts()
    {
        var productList = await productService.GetProductsAsync();

        return Ok(productList);
    }

    [HttpGet(":id")]
    public async Task<ActionResult<ProdutDetails>> GetProduct([FromRoute] Guid productId)
    {
        var product = await productService.GetProductAsync(productId);

        return Ok(product);
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult> FavoriteProduct([FromBody] Guid productId)
    {
        var userId = User.FindFirstValue("UserId");
        await productService.FavoriteProduct(productId, new Guid(userId!));
        return Ok();
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult> UnfavoriteProduct([FromBody] Guid productId)
    {
        var userId = User.FindFirstValue("UserId");
        await productService.UnfavoriteProduct(productId, new Guid(userId!));
        return Ok();
    }
}
