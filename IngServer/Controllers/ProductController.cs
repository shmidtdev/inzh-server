using System.Net.Mime;
using System.Text.RegularExpressions;
using IngServer.DataBase;
using IngServer.DataBase.Models;
using IngServer.Dtos;
using IngServer.Dtos.Products;
using IngServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

namespace IngServer.Controllers;

[Route("api/product/[action]")]
public class ProductController(
    ApplicationContext applicationContext,
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
    public CatalogPageContextDto GetProductsBySubstring(string substring, int page = 1)
    {
        var products = productRepository.GetBySubstring(substring);

        return new CatalogPageContextDto
        {
            Products = products.Skip(page).Take(25).ToList(),
            Pages = (products.Count() / 25) - 1,
        };
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
        return await productRepository.GetRecommendedProductsAsync(15);
    }
    
    [HttpGet]
    public async Task<List<Product>> GetActions()
    {
        return await productRepository.GetActionProductsAsync(15);
    }

    [HttpPost]
    public async Task<bool> AttachImages([FromBody] AttachImageDto? dto)
    {
        var bucketName = "productimages";
        var imageName = "image.png";

        if (dto is null)
            return false;
        
        var product = await productRepository.GetProductAsync(dto.ProductId);
        if (product is null)
            return false;
        
        var images = new List<Image>();
        
        foreach (var image in dto.Images)
        {
            var guid = Guid.NewGuid();
        
            Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
            
            var base64File = regex.Replace(image,string.Empty);
            var bytes = Convert.FromBase64String(base64File);
            
            using var fileStream = new MemoryStream(bytes);
            var poa = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject($"{guid}.jpg")
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(MediaTypeNames.Application.Octet);
            
            await minioClient.PutObjectAsync(poa);

            var imageObject = new Image
            {
                Id = guid,
                Link = $"{bucketName}/{guid}.jpg",
                UploadDate = DateTime.UtcNow,
                ProductId = product.Id
            };

            applicationContext.Images.Add(imageObject);
        }

        if (product.Images is null)
            product.Images = images;
        else 
            product.Images.AddRange(images);
        
        await applicationContext.SaveChangesAsync();
        
        return true;
    }
}