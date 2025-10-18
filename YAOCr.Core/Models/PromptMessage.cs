using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAOCr.Core.Models;
public record PromptMessage (
    string Message,
    List<string> FilePaths);
