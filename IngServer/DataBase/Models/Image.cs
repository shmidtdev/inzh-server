namespace IngServer.DataBase.Models;

/// <summary>
/// Изображение
/// </summary>
public class Image
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Ссылка на изображение
    /// </summary>
    public string Link { get; set; }
    
    /// <summary>
    /// Дата загрузки
    /// </summary>
    public DateTime UploadDate { get; set; }
    
    public Guid ProductId { get; set; }
}