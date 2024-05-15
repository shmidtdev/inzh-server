namespace IngServer.DataBase.Models;

public class WishList
{
    public Guid Id { get; set; }
    public User User { get; set; }
    public List<Product> Products { get; set; }
}