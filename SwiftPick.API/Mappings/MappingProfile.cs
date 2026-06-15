using AutoMapper;
using SwiftPick.Core.DTOs;
using SwiftPick.Core.Entities;

namespace SwiftPick.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name))
            .ForMember(d => d.Images, opt => opt.MapFrom(s => s.Images))
            .ForMember(d => d.Specifications, opt => opt.MapFrom(s => s.Specifications));

        CreateMap<ProductImage, ProductImageDto>();
        CreateMap<ProductSpecification, ProductSpecificationDto>();

        // Category mappings
        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.ParentCategoryName, opt => opt.MapFrom(s => s.ParentCategory != null ? s.ParentCategory.Name : null))
            .ForMember(d => d.SubCategories, opt => opt.MapFrom(s => s.SubCategories))
            .ForMember(d => d.ProductCount, opt => opt.MapFrom(s => s.Products.Count));

        // Cart mappings
        CreateMap<Cart, CartDto>()
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items))
            .ForMember(d => d.TotalAmount, opt => opt.MapFrom(s => s.Items.Sum(i => i.Product.Price * i.Quantity)));

        CreateMap<CartItem, CartItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name))
            .ForMember(d => d.ProductImagePath, opt => opt.MapFrom(s => s.Product.Images.FirstOrDefault() != null ? s.Product.Images.First().ImagePath : null))
            .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Product.Price));

        // Order mappings
        CreateMap<Order, OrderDto>()
            .ForMember(d => d.UserEmail, opt => opt.MapFrom(s => s.User != null ? s.User.Email : s.Email))
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name))
            .ForMember(d => d.ProductImagePath, opt => opt.MapFrom(s => s.Product.Images.FirstOrDefault() != null ? s.Product.Images.First().ImagePath : null));

        // User mappings
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(d => d.Roles, opt => opt.Ignore());
    }
}
