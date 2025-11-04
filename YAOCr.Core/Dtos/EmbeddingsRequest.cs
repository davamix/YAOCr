using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YAOCr.Core.Dtos;
public class EmbeddingRequest {
    [JsonPropertyName("input")]
    public string Content { get; set; }
    [JsonPropertyName("model")]
    public string ModelName { get; set; }
    [JsonPropertyName("encoding_format")]
    public string EncodingFormat { get; set; }
}
