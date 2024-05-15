using System.ComponentModel.DataAnnotations;

namespace IngServer.DataBase.Models;

/// <summary>
/// Категория товара
/// </summary>
public class Category
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название
    /// </summary>
    [MaxLength(100)]
    public required string Name { get; set; } = null!;

    /// <summary>
    /// Название (англ.)
    /// </summary>
    [MaxLength(100)]
    public required string NameEng { get; set; } = null!;
    
    /// <summary>
    /// Ссылка на изображение
    /// </summary>
    public Image? Image { get; set; }
    
    /// <summary>
    /// Подкатегории
    /// </summary>
    public List<Category>? Children { get; set; }
}