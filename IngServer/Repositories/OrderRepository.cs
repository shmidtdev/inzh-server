using IngServer.DataBase;
using IngServer.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace IngServer.Repositories;

public class OrderRepository(ApplicationContext applicationContext, ProductMovementRepository productMovementRepository)
{
    public async Task<Order> CreateAsync(User user)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            User = user,
            ProductMovements = new List<ProductMovement>(),
            LastUpdateDate = DateTime.UtcNow
        };

        await applicationContext.Orders.AddAsync(order);

        return order;
    }
    
    public async Task<Order> CreateAsync()
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            User = null,
            ProductMovements = new List<ProductMovement>(),
            LastUpdateDate = DateTime.UtcNow
        };

        await applicationContext.Orders.AddAsync(order);

        return order;
    }

    public async Task<Order?> GetAsync(Guid id)
    {
        return await applicationContext.Orders
            .Include(x => x.ProductMovements)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Order?> GetByUserAsync(User user)
    {
        return await applicationContext.Orders
            .Include(x => x.ProductMovements)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.User.Id == user.Id);
    }

    public async Task RemoveProductMovementAsync(Guid orderId, Guid productId)
    {
        var order = await applicationContext.Orders
            .Include(x => x.ProductMovements)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == orderId);
        
        if (order is null)
            return;
        
        var productMovement = order.ProductMovements.FirstOrDefault(x => x.Product.Id == productId);
        if (productMovement is null)
            return;
        
        order.ProductMovements.Remove(productMovement);
    }
}