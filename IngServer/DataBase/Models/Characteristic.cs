using System.ComponentModel.DataAnnotations;

namespace IngServer.DataBase.Models;

/// <summary>
/// Характеристики товара
/// </summary>
public class Characteristic
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Название зарактеристики
    /// </summary>
    [MaxLength(100)]
    public required string Name { get; set; }
    
    /// <summary>
    /// Значение зарактеристики
    /// </summary>
    [MaxLength(150)]
    public required string Value { get; set; }
    
    /// <summary>
    /// Название зарактеристики (англ.)
    /// </summary>
    [MaxLength(100)]
    public required string NameEng { get; set; }
    
    /// <summary>
    /// Значение зарактеристики (англ.)
    /// </summary>
    [MaxLength(150)]
    public required string ValueEng { get; set; }

    /// <summary>
    /// Список товаров с такой характеристикой
    /// </summary>
    public List<Product> Products { get; set; }
}