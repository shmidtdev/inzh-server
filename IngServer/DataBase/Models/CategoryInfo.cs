namespace IngServer.DataBase.Models;

public class CategoryInfo
{
    public Guid Id { get; set; }
    public Category Category { get; set; }
    public List<CategoryInfo> BottomChildren { get; set; }
    public List<Characteristic> Characteristics { get; set; }
    public int AmountOfProducts { get; set; }
}