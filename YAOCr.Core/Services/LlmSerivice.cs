using System;
using System.Collections.Generic;
using System.Linq;
using YAOCr.Core.Models;

namespace YAOCr.Core.Services;

public interface ILlmService {
    string CreateLlmMessage(string message, List<MessageAttachment> attachments);
}
public class LlmService : ILlmService {
    private readonly IFileStorageService _fileStorageService;

    public LlmService(IFileStorageService fileStorageService) {
        _fileStorageService = fileStorageService;
    }

    public string CreateLlmMessage(string message, List<MessageAttachment> attachments) {
        var content = message;
        content += "\n\nFILES ATTACHED\n";

        foreach (var file in attachments) {
            content += String.Format($"File: {file.Path}\n");
            content += "Content:\n";
            content += file.Content;
            content += "\n\n";
        }

        return content.Trim();
    }
}
