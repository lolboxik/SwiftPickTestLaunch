using SwiftPick.Core.DTOs;
using SwiftPick.Core.Entities;
using SwiftPick.Core.Interfaces;

namespace SwiftPick.Services.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartDto> GetCartAsync(string userId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        return MapToDto(cart ?? new Cart { UserId = userId });
    }

    public async Task<CartDto> AddToCartAsync(string userId, int productId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Количество должно быть больше 0");

        var cart = await _cartRepository.GetOrCreateAsync(userId);
        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null) throw new Exception("Product not found");

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            cart.Items.Add(new Core.Entities.CartItem
            {
                ProductId = productId,
                Quantity = quantity,
                CartId = cart.Id
            });
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _cartRepository.UpdateAsync(cart);
        return MapToDto(cart);
    }

    public async Task<CartDto> UpdateCartItemAsync(string userId, int productId, int quantity)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null) throw new Exception("Cart not found");

        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null) throw new Exception("Item not found in cart");

        if (quantity <= 0)
        {
            cart.Items.Remove(item);
        }
        else
        {
            item.Quantity = quantity;
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _cartRepository.UpdateAsync(cart);
        return MapToDto(cart);
    }

    public async Task<CartDto> RemoveFromCartAsync(string userId, int productId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null) throw new Exception("Cart not found");

        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Items.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateAsync(cart);
        }

        return MapToDto(cart);
    }

    public async Task<CartDto> ClearCartAsync(string userId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null) throw new Exception("Cart not found");

        cart.Items.Clear();
        cart.UpdatedAt = DateTime.UtcNow;
        await _cartRepository.UpdateAsync(cart);
        return MapToDto(cart);
    }

    private CartDto MapToDto(Cart cart)
    {
        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            CreatedAt = cart.CreatedAt,
            UpdatedAt = cart.UpdatedAt,
            Items = cart.Items.Select(i => new CartItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? string.Empty,
                ProductImagePath = i.Product?.Images?.FirstOrDefault()?.ImagePath,
                Price = i.Product?.Price ?? 0,
                Quantity = i.Quantity
            }).ToList(),
            TotalAmount = cart.Items.Sum(i => (i.Product?.Price ?? 0) * i.Quantity)
        };
    }
}
