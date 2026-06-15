using SwiftPick.Core.DTOs;
using SwiftPick.Core.Entities;
using SwiftPick.Core.Interfaces;

namespace SwiftPick.Services.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _repository.GetWithSubCategoriesAsync(id);
        return category == null ? null : MapToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            ParentCategoryId = dto.ParentCategoryId,
            ImagePath = dto.ImagePath
        };

        await _repository.CreateAsync(category);
        return MapToDto(category);
    }

    public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null) return null;

        if (dto.Name != null) category.Name = dto.Name;
        if (dto.Description != null) category.Description = dto.Description;
        if (dto.ParentCategoryId.HasValue) category.ParentCategoryId = dto.ParentCategoryId.Value;
        if (dto.ImagePath != null) category.ImagePath = dto.ImagePath;

        await _repository.UpdateAsync(category);
        return MapToDto(category);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null) return false;

        await _repository.DeleteAsync(category);
        return true;
    }

    private CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImagePath = category.ImagePath,
            ParentCategoryId = category.ParentCategoryId,
            ParentCategoryName = category.ParentCategory?.Name,
            SubCategories = category.SubCategories.Select(MapToDto).ToList(),
            ProductCount = category.Products.Count
        };
    }
}
