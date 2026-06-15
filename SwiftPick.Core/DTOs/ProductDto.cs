using System.ComponentModel.DataAnnotations;

namespace SwiftPick.Core.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImagePath { get; set; }
    public string? Brand { get; set; }
    public bool IsActive { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public List<ProductImageDto> Images { get; set; } = new();
    public List<ProductSpecificationDto> Specifications { get; set; } = new();
}

public class CreateProductDto
{
    [Required(ErrorMessage = "Название обязательно")]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
    public decimal Price { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Остаток не может быть отрицательным")]
    public int Stock { get; set; }
    [Required]
    public int CategoryId { get; set; }
    public string? Brand { get; set; }
    public List<string>? ImagePaths { get; set; }
    public List<SpecificationDto>? Specifications { get; set; }
}

public class UpdateProductDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? Stock { get; set; }
    public int? CategoryId { get; set; }
    public string? Brand { get; set; }
    public bool? IsActive { get; set; }
}

public class ProductImageDto
{
    public int Id { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsMain { get; set; }
}

public class ProductSpecificationDto
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class SpecificationDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
