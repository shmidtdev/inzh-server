using System.ComponentModel.DataAnnotations;
using IngServer.DataBase.Enums;

namespace IngServer.DataBase.Models;

/// <summary>
/// Пользователь
/// </summary>
public class User
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Почта
    /// </summary>
    [MaxLength(100)]
    public string Email { get; set; }
    
    /// <summary>
    /// Пароль
    /// </summary>
    [MaxLength(64)]
    public string Password { get; set; }
    
    /// <summary>
    /// Имя
    /// </summary>
    [MaxLength(100)]
    public string? Name { get; set; }
    
    /// <summary>
    /// Телефон
    /// </summary>
    [MaxLength(12)]
    public string? Phone { get; set; }
    
    /// <summary>
    /// Разрешена рассылка
    /// </summary>
    public bool IsMailAllowed { get; set; }
    
    /// <summary>
    /// Роль
    /// </summary>
    public UserRole UserRole { get; set; }
    
    /// <summary>
    /// Дата создания аккаунта
    /// </summary>
    public DateTime CreationDate { get; set; }
}