using System.Text.Json.Serialization;

namespace IngServer.Dtos;

public class CategoryDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("nameEng")]
    public string NameEng { get; set; }
    
    [JsonPropertyName("imageLink")]
    public string? ImageLink { get; set; }
    
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
}