using System.Text.Json.Serialization;
using IngServer.DataBase.Models;

namespace IngServer.Dtos;

public class ProductPageContextDto
{
    [JsonPropertyName("product")]
    public Product Product { get; set; }
    
    [JsonPropertyName("breadCrumbs")]
    public List<BreadCrumb> BreadCrumbs { get; set; }
}