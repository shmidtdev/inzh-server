using IngServer.DataBase.Enums;
using IngServer.DataBase.Models;

namespace IngServer.Dtos.Products;

public class ProductDto
{
    public Guid Id { get; set; }
    
    public required string Title { get; set; }
    
    public required string TitleEng { get; set; }

    public Category? Category { get; set; }

    public double OldPrice { get; set; }

    public double Price { get; set; }

    public List<Image>? Images { get; set; }

    public double Rating { get; set; }

    public ProductAvailability ProductAvailability { get; set; }

    public bool IsRecommended { get; set; }

    public List<Characteristic> Characteristics { get; set; } = new();
}