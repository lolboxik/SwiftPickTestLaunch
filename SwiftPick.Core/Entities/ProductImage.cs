namespace SwiftPick.Core.Entities;

public class ProductImage
{
    public int Id { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsMain { get; set; }
    
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
