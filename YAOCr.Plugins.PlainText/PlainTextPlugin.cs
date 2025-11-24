using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YAOCr.Plugins.Parsers;

namespace YAOCr.Plugins.PlainText;

public class PlainTextPlugin : IFileParser {
    public string Id => "PlainTextPlugin";
    public string Name => "Plain Text File Parser";
    public string Description => "A plugin to parse plain text files including .txt, .yaml, .json, .csv, and .sql formats.";
    public List<string> Extensions => [".txt", ".yaml", ".json", ".csv", ".sql"];

    public List<string> ContentTypes => ["text/plain", "application/json"];

    public async Task<string> Parse(string filePath) {
        if (string.IsNullOrEmpty(filePath)) {
            throw new ArgumentException("filePath cannot be empty");
        }

        if (!File.Exists(filePath)) {
            throw new FileNotFoundException("File not found", filePath);
        }

        var content = await File.ReadAllTextAsync(filePath);

        return content
            .Replace("\r\n", "\n")  // Normalize line endings
            .Replace("\r", "\n")    // Handle any remaining lone \r
            .Replace("\u0085", "\n") // Handle next line character
            .Replace("\u2028", "\n") // Handle line separator
            .Replace("\u2029", "\n"); // Handle paragraph separator;
    }
}
