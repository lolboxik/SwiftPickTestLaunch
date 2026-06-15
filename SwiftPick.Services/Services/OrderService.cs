using Microsoft.EntityFrameworkCore;
using SwiftPick.Core.DTOs;
using SwiftPick.Core.Entities;
using SwiftPick.Core.Interfaces;
using SwiftPick.Infrastructure.Data;

namespace SwiftPick.Services.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartService _cartService;
    private readonly IProductRepository _productRepository;
    private readonly ApplicationDbContext _dbContext;

    public OrderService(IOrderRepository orderRepository, ICartService cartService, IProductRepository productRepository, ApplicationDbContext dbContext)
    {
        _orderRepository = orderRepository;
        _cartService = cartService;
        _productRepository = productRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<OrderDto>> GetByUserIdAsync(string userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        return orders.Select(MapToDto);
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        return order == null ? null : MapToDto(order);
    }

    public async Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto dto)
    {
        var cart = await _cartService.GetCartAsync(userId);
        if (cart == null || !cart.Items.Any())
            throw new Exception("Cart is empty");

        // Проверяем наличие товаров на складе
        foreach (var item in cart.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new Exception($"Товар '{item.ProductName}' больше не доступен");
            if (product.Stock < item.Quantity)
                throw new Exception($"Недостаточно товара '{product.Name}' на складе (доступно: {product.Stock})");
        }

        var order = new Order
        {
            UserId = userId,
            OrderNumber = GenerateOrderNumber(),
            TotalAmount = cart.TotalAmount,
            ShippingAddress = dto.ShippingAddress,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            PaymentMethod = dto.PaymentMethod,
            Status = OrderStatus.New,
            Items = cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await _orderRepository.CreateAsync(order);
            await _cartService.ClearCartAsync(userId);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return MapToDto(order);
    }

    public async Task<OrderDto?> UpdateStatusAsync(int id, OrderStatus status)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) return null;

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        if (status == OrderStatus.Shipped)
            order.ShippedAt = DateTime.UtcNow;
        else if (status == OrderStatus.Delivered)
            order.DeliveredAt = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order);
        return MapToDto(order);
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllWithDetailsAsync();
        return orders.Select(MapToDto);
    }

    private string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";
    }

    private OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            UserId = order.UserId,
            UserEmail = order.User?.Email ?? order.Email,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            ShippingAddress = order.ShippingAddress,
            PhoneNumber = order.PhoneNumber,
            Email = order.Email,
            PaymentMethod = order.PaymentMethod,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            ShippedAt = order.ShippedAt,
            DeliveredAt = order.DeliveredAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? string.Empty,
                ProductImagePath = i.Product?.Images?.FirstOrDefault()?.ImagePath,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };
    }
}
