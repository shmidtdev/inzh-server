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
}