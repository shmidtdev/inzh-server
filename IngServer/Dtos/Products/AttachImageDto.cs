using System.Text.Json.Serialization;

namespace IngServer.Dtos.Products;

public class AttachImageDto
{
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }
    
    [JsonPropertyName("images")]
    public List<string> Images { get; set; }
}