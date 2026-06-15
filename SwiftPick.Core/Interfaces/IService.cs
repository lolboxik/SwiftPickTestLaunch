using SwiftPick.Core.DTOs;
using SwiftPick.Core.Entities;

namespace SwiftPick.Core.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductDto>> SearchAsync(string searchTerm);
    Task<IEnumerable<ProductDto>> GetFilteredAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, string? brand);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface ICartService
{
    Task<CartDto> GetCartAsync(string userId);
    Task<CartDto> AddToCartAsync(string userId, int productId, int quantity);
    Task<CartDto> UpdateCartItemAsync(string userId, int productId, int quantity);
    Task<CartDto> RemoveFromCartAsync(string userId, int productId);
    Task<CartDto> ClearCartAsync(string userId);
}

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetByUserIdAsync(string userId);
    Task<OrderDto?> GetByIdAsync(int id);
    Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto dto);
    Task<OrderDto?> UpdateStatusAsync(int id, OrderStatus status);
    Task<IEnumerable<OrderDto>> GetAllAsync();
}

public interface IAuthService
{
    Task<AuthResultDto> RegisterAsync(RegisterDto dto);
    Task<AuthResultDto> LoginAsync(LoginDto dto);
    Task<AuthResultDto> LoginWithProviderAsync(string provider, string providerId, string email, string? firstName, string? lastName);
    Task<AuthResultDto> ChangePasswordAsync(string userId, ChangePasswordDto dto);
}
