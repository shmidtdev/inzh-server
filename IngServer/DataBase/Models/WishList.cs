namespace IngServer.DataBase.Models;

public class WishList
{
    public Guid Id { get; set; }
    public User? User { get; set; }
    public List<ProductMovement> ProductMovements { get; set; }
}