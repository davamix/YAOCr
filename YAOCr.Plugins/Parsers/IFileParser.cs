using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAOCr.Plugins.Parsers;
public interface IFileParser : IPlugin {
    List<string> Extensions { get; }
    List<string> ContentTypes { get; }
    Task<string> Parse(string filePath);
}
