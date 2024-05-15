using IngServer.DataBase.Models;
using IngServer.Repositories;

namespace IngServer;

public class BreadCrumbManager(CategoryRepository categoryRepository)
{
    public async Task<List<BreadCrumb>> GetBreadCrumbs(string categoryName)
    {
        var breadCrumbs = new List<BreadCrumb>();

        await FillBreadCrumbList(breadCrumbs, categoryName);

        return breadCrumbs;
    }
    
    private async Task FillBreadCrumbList(List<BreadCrumb> breadCrumbs, string categoryName)
    {
        var category = await categoryRepository.GetCategoryAsync(categoryName);
        if (category is null)
            return;
        
        breadCrumbs.Add(new BreadCrumb
        {
            Name = category.Name,
            Slug = category.NameEng.ToLower() == "catalog" ? category.NameEng.ToLower() : $"catalog/{category.NameEng.ToLower()}"
        });

        var parent = await categoryRepository.GetParentAsync(category);
        if (parent is null)
            return;

        await FillBreadCrumbList(breadCrumbs, parent.NameEng);
    }
}