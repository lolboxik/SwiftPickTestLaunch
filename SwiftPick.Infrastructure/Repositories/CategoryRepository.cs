using Microsoft.EntityFrameworkCore;
using SwiftPick.Core.Entities;
using SwiftPick.Core.Interfaces;
using SwiftPick.Infrastructure.Data;

namespace SwiftPick.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetWithProductsAsync()
    {
        return await _dbSet
            .Include(c => c.Products.Where(p => p.IsActive))
            .Include(c => c.SubCategories)
            .Where(c => c.ParentCategoryId == null)
            .ToListAsync();
    }

    public async Task<Category?> GetWithSubCategoriesAsync(int id)
    {
        return await _dbSet
            .Include(c => c.SubCategories)
            .Include(c => c.Products.Where(p => p.IsActive))
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public override async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.SubCategories)
            .Include(c => c.ParentCategory)
            .Include(c => c.Products.Where(p => p.IsActive))
            .ToListAsync();
    }
}
