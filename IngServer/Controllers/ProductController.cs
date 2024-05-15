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
            products.AddRange(await productRepository.GetProductsAsync(category.NameEng));

        return products;
    }

    [HttpGet]
    public List<Product> GetProductsBySubstring(string substring)
    {
        return productRepository.GetBySubstring(substring);
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
}