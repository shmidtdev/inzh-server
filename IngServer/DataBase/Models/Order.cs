namespace IngServer.DataBase.Models;

/// <summary>
/// Заказ
/// </summary>
public class Order
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Пользователь, которому принадлежит заказ
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Список товаров
    /// </summary>
    public List<ProductMovement> ProductMovements { get; set; } = new ();
    
    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime LastUpdateDate { get; set; }
}