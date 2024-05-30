using IngServer.DataBase;
using IngServer.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace IngServer.Repositories;

public class CategoryRepository(ApplicationContext applicationContext)
{
    public async Task<Category?> GetParentAsync(Category category)
    {
        if (category?.Children is not null)
            return await applicationContext.Categories.FirstOrDefaultAsync(x => x.Children.Contains(category));

        return null;
    }
    
    public Task<Category?> GetCategoryAsync(string nameEng)
    {
        return applicationContext.Categories
            .Include(x => x.Children)
            .FirstOrDefaultAsync(x => x.NameEng.ToLower() == nameEng.ToLower());
    }

    public async Task<IQueryable<Category>?> GetBottomChildrenAsync(string nameEng)
    {
        var category = await applicationContext.Categories
            .Include(x => x.Children)
            .FirstOrDefaultAsync(x => x.NameEng.ToLower() == nameEng.ToLower());
        if (category is null)
            return null;
    
        return GetBottomChildrenAsync(category);
    }
    
    // public async Task<List<Category>> GetBottomChildrenAsync(Category category)
    // {
    //     var dbCategory = await GetCategoryAsync(category.NameEng);
    //     
    //     var children = new List<Category>();
    //
    //     if (dbCategory?.Children is null || dbCategory.Children?.Count == 0)
    //     {
    //         children.Add(category);
    //         return children;
    //     }
    //     
    //     foreach (var child in dbCategory.Children)
    //         children.AddRange(await GetBottomChildrenAsync(child));
    //
    //     return children;
    // }
    
    public IQueryable<Category> GetBottomChildrenAsync(Category category)
    {
        return applicationContext.CategoryInfos
            .Include(x => x.Category)
            .Include(x => x.BottomChildren)
            .Where(x => x.Category.Id == category.Id)
            .SelectMany(x => x.BottomChildren)
            .Select(x => x.Category);
    }
    
    public Task<CategoryInfo?> GetCategoryInfo(Category category)
    {
        return applicationContext.CategoryInfos.Include(x => x.Characteristics).FirstOrDefaultAsync(x => x.Category.Id == category.Id);
    }
}