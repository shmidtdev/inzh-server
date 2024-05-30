using System.Text.Json.Serialization;

namespace IngServer.Dtos;

public class CatalogPostDto
{
    [JsonPropertyName("categoryName")]
    public string CategoryName { get; set; }
    
    [JsonPropertyName("params")]
    public Dictionary<string, string> Params { get; set; }
    
    [JsonPropertyName("maxPrice")]
    public int MaxPrice { get; set; }
    
    [JsonPropertyName("minPrice")]
    public int MinPrice { get; set; }
    
    [JsonPropertyName("order")]
    public int Order { get; set; }
    
    [JsonPropertyName("page")]
    public int Page { get; set; }
}