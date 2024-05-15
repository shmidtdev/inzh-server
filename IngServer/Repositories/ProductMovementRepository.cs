using IngServer.DataBase;
using IngServer.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace IngServer.Repositories;

public class ProductMovementRepository(ApplicationContext applicationContext)
{
    public Task<ProductMovement?> GetAsync(Guid productId)
    {
        return applicationContext.ProductMovements.FirstOrDefaultAsync(x => x.Product.Id == productId);
    }
}