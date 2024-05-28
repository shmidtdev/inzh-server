using IngServer.DataBase;
using IngServer.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace IngServer.Repositories;

public class WishListRepository(ApplicationContext applicationContext)
{
    public async Task<WishList> CreateAsync(User? user)
    {
        var wishList = new WishList
        {
            Id = default,
            User = user,
            ProductMovements = new List<ProductMovement>()
        };

        await applicationContext.WishLists.AddAsync(wishList);

        return wishList;
    }
    
    public async Task<WishList?> GetAsync(Guid id)
    {
        return await applicationContext.WishLists.Include(x => x.ProductMovements).ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<WishList?> GetByUserAsync(User user)
    {
        return await applicationContext.WishLists
            .Include(x => x.ProductMovements)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.User.Id == user.Id);
    }
    
    public async Task RemoveProductMovementAsync(Guid wishListId, Guid productId)
    {
        var wishList = await applicationContext.WishLists
            .Include(x => x.ProductMovements)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == wishListId);
        
        if (wishList is null)
            return;
        
        var productMovement = wishList.ProductMovements.FirstOrDefault(x => x.Product.Id == productId);
        if (productMovement is null)
            return;
        
        wishList.ProductMovements.Remove(productMovement);
    }
}