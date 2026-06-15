using AutoMapper;
using SwiftPick.Core.DTOs;
using SwiftPick.Core.Entities;
using SwiftPick.Core.Interfaces;

namespace SwiftPick.Services.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository repository, ICategoryRepository categoryRepository, IMapper mapper)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(p => MapToDto(p));
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId)
    {
        var products = await _repository.GetByCategoryAsync(categoryId);
        return products.Select(p => MapToDto(p));
    }

    public async Task<IEnumerable<ProductDto>> SearchAsync(string searchTerm)
    {
        var products = await _repository.SearchAsync(searchTerm);
        return products.Select(p => MapToDto(p));
    }

    public async Task<IEnumerable<ProductDto>> GetFilteredAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, string? brand)
    {
        var products = await _repository.GetFilteredAsync(categoryId, minPrice, maxPrice, brand);
        return products.Select(p => MapToDto(p));
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category == null)
            throw new Exception("Указанная категория не существует");

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId,
            Brand = dto.Brand,
            IsActive = true
        };

        if (dto.ImagePaths != null && dto.ImagePaths.Any())
        {
            for (int i = 0; i < dto.ImagePaths.Count; i++)
            {
                product.Images.Add(new ProductImage
                {
                    ImagePath = dto.ImagePaths[i],
                    SortOrder = i,
                    IsMain = i == 0
                });
            }
        }

        if (dto.Specifications != null)
        {
            foreach (var spec in dto.Specifications)
            {
                product.Specifications.Add(new ProductSpecification
                {
                    Key = spec.Key,
                    Value = spec.Value
                });
            }
        }

        await _repository.CreateAsync(product);
        return MapToDto(product);
    }

    public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return null;

        if (dto.Name != null) product.Name = dto.Name;
        if (dto.Description != null) product.Description = dto.Description;
        if (dto.Price.HasValue) product.Price = dto.Price.Value;
        if (dto.Stock.HasValue) product.Stock = dto.Stock.Value;
        if (dto.CategoryId.HasValue) product.CategoryId = dto.CategoryId.Value;
        if (dto.Brand != null) product.Brand = dto.Brand;
        if (dto.IsActive.HasValue) product.IsActive = dto.IsActive.Value;

        await _repository.UpdateAsync(product);
        return MapToDto(product);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return false;

        await _repository.DeleteAsync(product);
        return true;
    }

    private ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            ImagePath = product.ImagePath,
            Brand = product.Brand,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            Images = product.Images.Select(i => new ProductImageDto
            {
                Id = i.Id,
                ImagePath = i.ImagePath,
                SortOrder = i.SortOrder,
                IsMain = i.IsMain
            }).ToList(),
            Specifications = product.Specifications.Select(s => new ProductSpecificationDto
            {
                Id = s.Id,
                Key = s.Key,
                Value = s.Value
            }).ToList()
        };
    }
}
