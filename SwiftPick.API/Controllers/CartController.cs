using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftPick.Core.DTOs;
using SwiftPick.Core.Interfaces;

namespace SwiftPick.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var userId = GetUserId();
        var cart = await _cartService.GetCartAsync(userId);
        return Ok(cart);
    }

    [HttpPost("add")]
    public async Task<ActionResult<CartDto>> AddToCart([FromBody] AddToCartDto dto)
    {
        var userId = GetUserId();
        var cart = await _cartService.AddToCartAsync(userId, dto.ProductId, dto.Quantity);
        return Ok(cart);
    }

    [HttpPut("update")]
    public async Task<ActionResult<CartDto>> UpdateCartItem([FromBody] UpdateCartItemDto dto)
    {
        var userId = GetUserId();
        var cart = await _cartService.UpdateCartItemAsync(userId, dto.ProductId, dto.Quantity);
        return Ok(cart);
    }

    [HttpDelete("remove/{productId}")]
    public async Task<ActionResult<CartDto>> RemoveFromCart(int productId)
    {
        var userId = GetUserId();
        var cart = await _cartService.RemoveFromCartAsync(userId, productId);
        return Ok(cart);
    }

    [HttpDelete("clear")]
    public async Task<ActionResult<CartDto>> ClearCart()
    {
        var userId = GetUserId();
        var cart = await _cartService.ClearCartAsync(userId);
        return Ok(cart);
    }
}
