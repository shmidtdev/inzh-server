using System.Text.Json.Serialization;

namespace IngServer.Dtos;

public class CatalogPostDto
{
    [JsonPropertyName("categoryName")]
    public string CategoryName { get; set; }
    
    [JsonPropertyName("params")]
    public Dictionary<string, string> Params { get; set; }
    
    [JsonPropertyName("priceMin")]
    public int PriceMin { get; set; }
    
    [JsonPropertyName("priceMax")]
    public int PriceMax { get; set; }
    
    [JsonPropertyName("page")]
    public int Page { get; set; }
}