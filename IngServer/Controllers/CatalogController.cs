﻿using System.IO.Hashing;
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
    BreadCrumbManager breadCrumbManager)
{
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
            var categoryInfo = await categoryRepository.GetCategoryInfo(child);
            
            children.Add(new CategoryDto
            {
                Id = child.Id,
                Name = child.Name,
                NameEng = child.NameEng,
                ImageLink = child.Image?.Link,
                Amount = categoryInfo?.AmountOfProducts ?? 0
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
        var bottomChildrenCategories = categoryRepository.GetBottomChildrenAsync(category);

        var characteristics = productRepository.GetProducts(bottomChildrenCategories.Select(x => x.NameEng).ToList())
            .SelectMany(x => x.Characteristics)
            .OrderBy(x => x.ValueEng)
            .AsEnumerable()
            .DistinctBy(x => new { x.NameEng, x.ValueEng })
            .Select(x => x.WithoutProducts())
            .ToList();
        
        var products = productRepository
            .GetProducts(bottomChildrenCategories.Select(x => x.NameEng).ToList(), dto)
            .ToList();

        var categoryInfo = await categoryRepository.GetCategoryInfo(category);

        var pages = (products.Count / productsAmountOnPage) + 1;

        var maxPrice = products?.OrderBy(x => x.Price)?.Last()?.Price ?? 0;
        
        var breadCrumbs = await breadCrumbManager.GetBreadCrumbs(dto.CategoryName);
        
        return new CatalogPageContextDto
        {
            Characteristics = characteristics,
            Products = products?.Select(x => x.WithoutCharacteristics()).OrderBy(x => x.TitleEng).Skip((currentPage - 1) * productsAmountOnPage).Take(productsAmountOnPage).ToList() ?? new List<Product>(),
            Categories = topChildrenCategories ?? [],
            CurrentCategory = category,
            BreadCrumbs = breadCrumbs,
            Pages = pages,
            MaxPrice = maxPrice
        };
    }

    [HttpPost]
    public async Task<CatalogPageContextDto> GetActions([FromBody]CatalogPostDto dto)
    {
        const int productsAmountOnPage = 24;

        var currentPage = 1;
        if (dto.Page > 1)
            currentPage = dto.Page;

        var products = await productRepository.GetActionProductsAsync();

        var pages = (products.Count / productsAmountOnPage) + 1;

        var characteristics = products.SelectMany(x => x.Characteristics).Select(x => x.WithoutProducts()).ToList();
        
        return new CatalogPageContextDto
        {
            Characteristics = characteristics,
            Products = products.Select(x => x.WithoutCharacteristics()).OrderBy(x => x.TitleEng).Skip((currentPage - 1) * productsAmountOnPage).Take(productsAmountOnPage).ToList(),
            Pages = pages,
        };
    }
}