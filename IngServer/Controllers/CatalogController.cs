using IngServer.DataBase.Extensions;
using IngServer.DataBase.Models;
using IngServer.Dtos;
using IngServer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace IngServer.Controllers;

[Route("api/catalog/[action]")]
public class CatalogController(
    CategoryRepository categoryRepository,
    ProductRepository productRepository,
    CharacteristicRepository characteristicRepository,
    BreadCrumbManager breadCrumbManager)
{
    // [HttpGet]
    // public async Task<CategoryDto> GetSingleCategoryInfo([FromQuery] string categoryName)
    // {
    //     var category = await categoryRepository.GetCategoryAsync(categoryName);
    //     if (category is null)
    //         return new CategoryDto();
    //
    //     var children = await categoryRepository.GetBottomChildrenAsync(category);
    //
    //     var amount = 0;
    //     foreach (var child in children)
    //         amount += await productRepository.GetAmountAsync(child);
    //     
    //     return new CategoryDto
    //     {
    //         Category = category.WithoutChildren(),
    //         Amount = amount
    //     };
    // }

    [HttpGet]
    public async Task<List<CategoryDto>?> GetCategoryChildrenInfo([FromQuery] string categoryName)
    {
        var children = new List<CategoryDto>();
        
        var category = await categoryRepository.GetCategoryAsync(categoryName);
        if (category is null)
            return null;

        if (category.Children is null)
            return null;
        
        foreach (var child in category.Children)
        {
            var subChildren = await categoryRepository.GetBottomChildrenAsync(child);
            
            var amount = subChildren.Sum(productRepository.GetAmount);
            
            children.Add(new CategoryDto
            {
                Id = child.Id,
                Name = child.Name,
                NameEng = child.NameEng,
                ImageLink = child.Image?.Link,
                Amount = amount
            });
        }

        return children;
    }

    //todo: оптимизировать, сейчас достает в память все продукты, потом сортирует
    [HttpPost]
    public async Task<CatalogPageContextDto> GetCatalog([FromBody]CatalogPostDto dto)
    {
        const int productsAmountOnPage = 24;

        var currentPage = 1;
        if (dto.Page > 1)
            currentPage = dto.Page;
        
        var category = await categoryRepository.GetCategoryAsync(dto.CategoryName);

        var topChildrenCategories = category.Children;
        var bottomChildrenCategories = await categoryRepository.GetBottomChildrenAsync(category);
        
        var products = new List<Product>();
        foreach (var child in bottomChildrenCategories)
            products.AddRange(await productRepository.GetProductsAsync(child.NameEng));

        var characteristics = products.SelectMany(characteristicRepository.GetCharacteristics).Distinct().ToList();

        var pages = (products.Count / productsAmountOnPage) + 1;

        var maxPrice = products.OrderBy(x => x.Price).Last().Price;

        var breadCrumbs = await breadCrumbManager.GetBreadCrumbs(dto.CategoryName);
        
        return new CatalogPageContextDto
        {
            Characteristics = characteristics,
            Products = products.OrderBy(x => x.TitleEng).Skip((currentPage - 1) * productsAmountOnPage).Take(productsAmountOnPage).ToList(),
            Categories = topChildrenCategories ?? [],
            CurrentCategory = category,
            BreadCrumbs = breadCrumbs,
            Pages = pages,
            MaxPrice = maxPrice
        };
    }
}