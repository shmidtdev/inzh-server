namespace IngServer.DataBase.Enums;

/// <summary>
/// Наличие
/// </summary>
public enum ProductAvailability
{
    /// <summary>
    /// В наличии
    /// </summary>
    InStock = 0,
    
    /// <summary>
    /// Под заказ
    /// </summary>
    ToOrder = 1,
    
    /// <summary>
    /// Нет в наличии
    /// </summary>
    OutOfStock = 2
}