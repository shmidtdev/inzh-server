namespace IngServer.DataBase.Models;

/// <summary>
/// Статья
/// </summary>
public class Article
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public required Guid Id { get; set; }
    
    /// <summary>
    /// Название
    /// </summary>
    public required string Title { get; set; }
    
    /// <summary>
    /// Описание
    /// </summary>
    public required string Description { get; set; }
    
    /// <summary>
    /// Изображение
    /// </summary>
    public required Image Image { get; set; }
    
    /// <summary>
    /// Наполнение страницы
    /// </summary>
    public required string Content { get; set; }
}