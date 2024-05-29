using System.Text.Json.Serialization;

namespace IngServer.Dtos.Mail;

public class SubscribeDto
{
    [JsonPropertyName("email")]
    public string Email { get; set; }
}