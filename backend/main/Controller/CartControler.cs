using System.Security.Claims;
using main.DTO.cart;
using main.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace main.Controller;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class CartController(CartService cartService) : ControllerBase
{
    private readonly CartService _cartService = cartService;

    [HttpGet]
    public async Task<ActionResult<CartOverView>> GetCart()
    {
        var userId = User.FindFirstValue("UserId");
        return await _cartService.GetCart(new Guid(userId!));
    }

    [HttpPost]
    public async Task<ActionResult<CartOverView>> CreateCart()
    {
        var userId = User.FindFirstValue("UserId");
        return await _cartService.CreateCart(new Guid(userId!));
    }

    [HttpPut("product")]
    public async Task<ActionResult<CartOverView>> AddToCart([FromBody] CartProductRequest cart)
    {
        return await _cartService.AddToCart(cart);
    }

    [HttpDelete("product")]
    public async Task<ActionResult<CartOverView>> RemoveFromCart([FromBody] CartProductRequest cart)
    {
        return await _cartService.RemoveFromCart(cart);
    }

    [HttpPost("{cartId}")]
    public async Task<ActionResult> PayCart([FromRoute] Guid cartId)
    {
        await _cartService.PayCart(cartId);

        return Ok("Payment successfull!");
    }
}
