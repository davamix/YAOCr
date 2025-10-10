using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAOCr.Core.Models;
public class Settings {
    public string Theme { get; set; } = string.Empty;
    public string QdrantServerAddress { get; set; } = string.Empty;
    public string CompletionAddress { get; set; } = string.Empty;
    public string ChatAddress { get; set; } = string.Empty;
    public string EmbeddingsAddress { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
}
