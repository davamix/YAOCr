using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAOCr.Core.Models;
public class Settings {
    public string Theme { get; set; } = string.Empty;
    public string OllamaServerAddress { get; set; } = string.Empty;
    public string QdrantServerAddress { get; set; } = string.Empty;
    public string LlmModelName { get; set; } = string.Empty;
}
