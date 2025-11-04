using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YAOCr.Core.Dtos;

public class SendMessageRequest {
    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;
    [JsonPropertyName("model")]
    public string Model { get; set; }
    [JsonPropertyName("messages")]
    public List<MessageDto> Messages { get; set; } = new();
}

public class SendMessageResponse {
    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; }
    [JsonPropertyName("role")]
    public string Role { get; set; }
    [JsonPropertyName("content")]
    public string Content { get; set; }
}