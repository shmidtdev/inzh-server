namespace IngServer.DataBase.Models;

public class ProductMovement
{
    public Guid Id { get; set; }
    public Product Product { get; set; }
    public DateTime CreationDate { get; set; }
}