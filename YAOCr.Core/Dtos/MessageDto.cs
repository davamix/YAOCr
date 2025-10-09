using System.Text.Json.Serialization;

namespace YAOCr.Core.Dtos;

public class MessageDto {
    [JsonPropertyName("role")]
    public string Role { get; set; }
    [JsonPropertyName("content")]
    public string Content { get; set; }
}
