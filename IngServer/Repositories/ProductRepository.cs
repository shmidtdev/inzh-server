using IngServer.DataBase;
using IngServer.DataBase.Enums;
using IngServer.DataBase.Extensions;
using IngServer.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace IngServer.Repositories;

public class ProductRepository(ApplicationContext applicationContext, CharacteristicRepository characteristicRepository)
{
    public int GetAmount(Category category)
    {
        return applicationContext.Products
            .Include(x => x.Category)
            .Count(x => x.Category == category);
    }
    
    public Task<int> GetAmountAsync(Category category)
    {
        return applicationContext.Products.Include(x => x.Category)
            .Where(x => x.Category == category)
            .CountAsync();
    }

    /// <summary>
    /// Получить товары по названию категории
    /// </summary>
    /// <param name="categoryName">Название категории</param>
    public Task<List<Product>> GetProductsAsync(string categoryName)
    {
        return applicationContext.Products.Include(x => x.Category)
            .Where(x => x.Category.NameEng == categoryName).ToListAsync();
    }

    public Task<Product?> GetProductAsync(Guid id)
    {
        return applicationContext.Products
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    //todo: поиск по названиям категорий
    public List<Product> GetBySubstring(string substring)
    {
        var products = applicationContext.Products.Where(x => x.SearchVector.Matches(substring)).ToList();

        var smt = products.Select(x => new Product
        {
            Id = x.Id,
            Title = x.Title,
            TitleEng = x.TitleEng,
            Category = x.Category,
            OldPrice = x.OldPrice,
            Price = x.Price,
            Images = x.Images,
            Rating = x.Rating,
            ProductAvailability = x.ProductAvailability,
            IsRecommended = x.IsRecommended,
            Characteristics = characteristicRepository.GetCharacteristics(x).Select(y => y.WithoutProducts()).ToList(),
            SearchVector = x.SearchVector,
            Orders = x.Orders,
            WishLists = x.WishLists
        });
        
        return smt.ToList();
    }
}