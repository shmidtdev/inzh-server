using IngServer.DataBase;
using IngServer.DataBase.Extensions;
using IngServer.DataBase.Models;
using IngServer.Dtos;
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
    public IQueryable<Product> GetProducts(string categoryName)
    {
        return applicationContext.Products.Include(x => x.Category)
            .Where(x => x.Category.NameEng == categoryName);
    }

    public IEnumerable<Product> GetProducts(string categoryName, CatalogPostDto dto)
    {
        var parameters = characteristicRepository
            .GetCharacteristics(x => dto.Params.ContainsKey(x.NameEng) 
                                     && dto.Params.ContainsValue(x.ValueEng));
        
        var max = dto.PriceMax > 0 ? dto.PriceMax : int.MaxValue;

        if (parameters.Count > 0)
        {
            return applicationContext.Products
                .Where(x => x.Category.NameEng.ToLower() == categoryName.ToLower())
                .Where(x => x.Price >= dto.PriceMin)
                .Where(x => x.Price <= max)
                .Where(x => x.Characteristics.Any(y => parameters.Contains(y)));
        }

        return applicationContext.Products
            .Where(x => x.Category.NameEng.ToLower() == categoryName.ToLower())
            .Where(x => x.Price >= dto.PriceMin)
            .Where(x => x.Price <= max);
    }
    
    public IEnumerable<Product> GetProducts(List<string> categoryNames, CatalogPostDto dto)
    {
        var parameters = characteristicRepository
            .GetCharacteristics(x => dto.Params.ContainsKey(x.NameEng) 
                                     && dto.Params.ContainsValue(x.ValueEng));
        
        var max = dto.PriceMax > 0 ? dto.PriceMax : int.MaxValue;

        if (parameters.Count > 0)
        {
            return applicationContext.Products
                .Include(x => x.Characteristics)
                .Where(x => categoryNames.Select(x => x.ToLower()).Contains(x.Category.NameEng.ToLower()))
                .Where(x => x.Price >= dto.PriceMin)
                .Where(x => x.Price <= max)
                .Where(x => x.Characteristics.Any(y => parameters.Contains(y)));
        }

        return applicationContext.Products
            .Include(x => x.Characteristics)
            .Where(x => categoryNames.Select(x => x.ToLower()).Contains(x.Category.NameEng.ToLower()))
            .Where(x => x.Price >= dto.PriceMin)
            .Where(x => x.Price <= max);
    }

    public Task<Product?> GetProductAsync(Guid id)
    {
        return applicationContext.Products
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public List<Product> GetBySubstring(string substring, int page = 1)
    {
        var products = applicationContext.Products.Where(x => x.SearchVector.Matches(substring)).Skip(page - 1).Take(25).ToList();

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
            Characteristics = characteristicRepository.GetCharacteristics(x).ToList(),
            SearchVector = x.SearchVector,
            Orders = x.Orders
        });
        
        return smt.ToList();
    }

    public Task<List<Product>> GetRecommendedProductsAsync()
    {
        return applicationContext.Products.Where(x => x.IsRecommended).Take(15).ToListAsync();
    }

    public Task<List<Product>> GetActionProductsAsync()
    {
        return applicationContext.Products.Where(x => x.OldPrice > 0).ToListAsync();
    }
    
    public Task<List<Product>> GetActionProductsAsync(int amount)
    {
        return applicationContext.Products.Where(x => x.OldPrice > 0).Take(amount).ToListAsync();
    }
}