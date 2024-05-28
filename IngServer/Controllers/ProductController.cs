using IngServer.DataBase.Models;
using IngServer.Dtos;
using IngServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

namespace IngServer.Controllers;

[Route("api/product/[action]")]
public class ProductController(
    ProductRepository productRepository,
    CategoryRepository categoryRepository,
    CharacteristicRepository characteristicRepository,
    BreadCrumbManager breadCrumbManager,
    IMinioClient minioClient)
{
    //todo: сюда дто
    [HttpGet]
    public async Task<IEnumerable<Product>?> GetProducts([FromQuery]string categoryName)
    {
        var categories = await categoryRepository.GetBottomChildrenAsync(categoryName);
        if (categories is null)
            return null;
        
        var products = new List<Product>();
        
        foreach (var category in categories)
            products.AddRange(productRepository.GetProducts(category.NameEng));

        return products;
    }

    [HttpGet]
    public List<Product> GetProductsBySubstring(string substring, int page = 1)
    {
        return productRepository.GetBySubstring(substring, page);
    }

    [HttpGet]
    public async Task<ProductPageContextDto> GetProduct(Guid id)
    {
        var product = await productRepository.GetProductAsync(id);
        if (product is null)
            return null;

        var characteristics = characteristicRepository.GetCharacteristics(product);
        product.Characteristics = characteristics;
        
        var breadCrumbs = await breadCrumbManager.GetBreadCrumbs(product.Category.NameEng);

        return new ProductPageContextDto
        {
            Product = product,
            BreadCrumbs = breadCrumbs
        };
    }

    [HttpGet]
    public async Task<List<Product>> GetRecommended()
    {
        return await productRepository.GetRecommendedProductsAsync();
    }
    
    [HttpGet]
    public async Task<List<Product>> GetActions()
    {
        return await productRepository.GetActionProductsAsync(15);
    }
}