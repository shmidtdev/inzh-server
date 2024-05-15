using System.Text.Json.Serialization;
using IngServer.DataBase.Models;

namespace IngServer.Dtos;

public class CatalogPageContextDto
{
    [JsonPropertyName("characteristics")]
    public List<Characteristic> Characteristics { get; set; }
    
    [JsonPropertyName("products")]
    public List<Product> Products { get; set; }
    
    [JsonPropertyName("categories")]
    public List<Category> Categories { get; set; }
    
    [JsonPropertyName("currentCategory")]
    public Category CurrentCategory { get; set; }
    
    [JsonPropertyName("breadCrumbs")]
    public List<BreadCrumb> BreadCrumbs { get; set; }
    
    [JsonPropertyName("pages")]
    public int Pages { get; set; }
    
    [JsonPropertyName("maxPrice")]
    public double MaxPrice { get; set; }
}