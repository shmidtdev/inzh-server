using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using IngServer.DataBase.Enums;
using NpgsqlTypes;

namespace IngServer.DataBase.Models;

/// <summary>
/// Товар
/// </summary>
public class Product
{
    /// <summary>
    /// Иентификатор
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Название
    /// </summary>
    [MaxLength(150)]
    public required string Title { get; set; }
    
    /// <summary>
    /// Название (англ.)
    /// </summary>
    [MaxLength(150)]
    public required string TitleEng { get; set; }
    
    /// <summary>
    /// Категория
    /// </summary>
    [MaxLength(150)]
    public required Category Category { get; set; }
    
    /// <summary>
    /// Старая цена
    /// </summary>
    public double OldPrice { get; set; }
    
    /// <summary>
    /// Цена
    /// </summary>
    public double Price { get; set; }
    
    /// <summary>
    /// Ссылки на фото
    /// </summary>
    public List<Image>? Images { get; set; }
    
    /// <summary>
    /// Рейтинг
    /// </summary>
    public double Rating { get; set; }
    
    /// <summary>
    /// Статус наличия
    /// </summary>
    public ProductAvailability ProductAvailability { get; set; }
    
    /// <summary>
    /// Рекомендованый продукт
    /// </summary>
    public bool IsRecommended { get; set; }

    /// <summary>
    /// Характеристики
    /// </summary>
    public List<Characteristic> Characteristics { get; set; } = new();
    
    /// <summary>
    /// Вектор полнотекстового поиска
    /// </summary>
    [JsonIgnore]
    public NpgsqlTsVector SearchVector { get; set; }
    
    [JsonIgnore]
    public List<Order> Orders { get; set; }
    
    [JsonIgnore]
    public List<WishList> WishLists { get; set; }
}