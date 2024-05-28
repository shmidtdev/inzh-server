using System.Text.Json.Serialization;

namespace IngServer.Dtos.Order;

public class OrderCommitItem
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("amountOfElements")]
    public int AmountOfElements { get; set; }
}