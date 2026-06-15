using Microsoft.EntityFrameworkCore;
using SwiftPick.Core.Entities;
using SwiftPick.Core.Interfaces;
using SwiftPick.Infrastructure.Data;

namespace SwiftPick.Infrastructure.Repositories;

public class CartRepository : Repository<Cart>, ICartRepository
{
    public CartRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Cart?> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Cart> GetOrCreateAsync(string userId)
    {
        var cart = await GetByUserIdAsync(userId);
        if (cart == null)
        {
            try
            {
                cart = new Cart { UserId = userId };
                await CreateAsync(cart);
            }
            catch (DbUpdateException)
            {
                // Параллельный запрос уже создал корзину
                cart = await GetByUserIdAsync(userId);
                if (cart == null) throw;
            }
        }
        return cart;
    }
}
