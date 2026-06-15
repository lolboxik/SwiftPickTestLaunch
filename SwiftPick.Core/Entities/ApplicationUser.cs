using Microsoft.AspNetCore.Identity;

namespace SwiftPick.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarPath { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Provider { get; set; }  // Google, VK, Yandex, Local
    public string? ProviderId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public Cart? Cart { get; set; }
}
