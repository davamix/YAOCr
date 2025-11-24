using System.Collections.Generic;

namespace YAOCr.Core.Models;
public record PromptMessage (
    string Message,
    List<FileParserContent> FileParsersContent);
