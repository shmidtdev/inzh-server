using IngServer.DataBase.Enums;

namespace IngServer.Dtos;

public class UserContext
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool IsAuthorized { get; set; }
    public UserRole UserRole { get; set; }
}