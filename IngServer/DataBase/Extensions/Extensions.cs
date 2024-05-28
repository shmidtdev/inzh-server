using IngServer.DataBase.Models;

namespace IngServer.DataBase.Extensions;

public static class Extensions
{
    public static Category WithoutChildren(this Category category)
    {
        return new Category
        {
            Id = category.Id,
            Name = category.Name,
            NameEng = category.NameEng,
            Image = category.Image,
            Children = null
        };
    }
    
    public static Characteristic WithoutProducts(this Characteristic characteristic)
    {
        return new Characteristic
        {
            Id = characteristic.Id,
            Name = characteristic.Name,
            Value = characteristic.Value,
            NameEng = characteristic.NameEng,
            ValueEng = characteristic.ValueEng,
            Products = null
        };
    }
    
    public static Product WithoutCharacteristics(this Product product)
    {
        return new Product
        {
            Id = product.Id,
            Title = product.Title,
            TitleEng = product.TitleEng,
            Category = product.Category,
            OldPrice = product.OldPrice,
            Price = product.Price,
            Images = product.Images,
            Rating = product.Rating,
            ProductAvailability = product.ProductAvailability,
            IsRecommended = product.IsRecommended,
            Characteristics = new List<Characteristic>(),
            SearchVector = product.SearchVector,
            Orders = product.Orders,
            WishLists = product.WishLists
        };
    }

    // public static ProductDto ToDto(this Product product)
    // {
    //     return new ProductDto
    //     {
    //         Id = product.Id,
    //         Title = product.Title,
    //         TitleEng = product.TitleEng,
    //         Category = product.Category,
    //         OldPrice = product.OldPrice,
    //         Price = product.Price,
    //         Images = product.Images,
    //         Rating = product.Rating,
    //         ProductAvailability = product.ProductAvailability,
    //         IsRecommended = product.IsRecommended,
    //         Characteristics = product.Characteristics,
    //     };
    // }
}