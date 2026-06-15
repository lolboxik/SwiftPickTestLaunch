using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SwiftPick.Core.DTOs;
using SwiftPick.Core.Entities;
using SwiftPick.Core.Interfaces;

namespace SwiftPick.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IProductService _productService;
    private readonly IOrderService _orderService;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IProductService productService,
        IOrderService orderService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _productService = productService;
        _orderService = orderService;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = _userManager.Users.ToList();
        var result = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AvatarPath = user.AvatarPath,
                Provider = user.Provider,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive,
                Roles = roles.ToList()
            });
        }

        return Ok(result);
    }

    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateUserRole(string id, [FromBody] UpdateRoleDto dto)
    {
        if (!await _roleManager.RoleExistsAsync(dto.Role))
        {
            return BadRequest(new { message = "Указанная роль не существует" });
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, dto.Role);

        return Ok(new { message = "Роль обновлена" });
    }

    [HttpPut("users/{id}/activate")]
    public async Task<IActionResult> ActivateUser(string id, [FromBody] ActivateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.IsActive = dto.IsActive;
        await _userManager.UpdateAsync(user);

        return Ok(new { message = "Статус обновлён" });
    }

    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<string>>> GetAllRoles()
    {
        var roles = _roleManager.Roles.Select(r => r.Name).Where(n => n != null).ToList();
        return Ok(roles);
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
    {
        var usersCount = _userManager.Users.Count();
        var activeUsers = _userManager.Users.Count(u => u.IsActive);

        var products = await _productService.GetAllAsync();
        var orders = await _orderService.GetAllAsync();

        var stats = new DashboardStatsDto
        {
            TotalUsers = usersCount,
            ActiveUsers = activeUsers,
            TotalProducts = products.Count(),
            TotalOrders = orders.Count(),
            Revenue = orders.Sum(o => o.TotalAmount)
        };

        return Ok(stats);
    }
}

public class UpdateRoleDto
{
    public string Role { get; set; } = string.Empty;
}

public class ActivateUserDto
{
    public bool IsActive { get; set; }
}

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public decimal Revenue { get; set; }
}
