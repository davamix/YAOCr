using System;
using System.Collections.Generic;
using System.Linq;

namespace YAOCr.Core.Services;

public interface ILlmService {
    string CreateLlmMessage(string message, List<(string Path, string Content)> filesContent);
}
public class LlmService : ILlmService {
    private readonly IFileStorageService _fileStorageService;

    public LlmService(IFileStorageService fileStorageService) {
        _fileStorageService = fileStorageService;
    }

    public string CreateLlmMessage(string message, List<(string Path, string Content)> filesContent) {
        var content = message;
        content += "\n\nFILES ATTACHED\n";

        foreach (var file in filesContent) {
            content += String.Format($"File: {file.Path}\n");
            content += "Content:\n";
            content += file.Content;
            content += "\n\n";
        }

        return content.Trim();
    }
}
