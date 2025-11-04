using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAOCr.Core.Extensions;
using YAOCr.Core.Models;

namespace YAOCr.Core.Services;

public interface IFileStorageService {
    Task ExportConversation(Conversation conversation, string filePath);
    Task<Conversation> ImportConversation(string filePath);
    Task<string>ReadTextFile(string filePath);
}

public class FileStorageService : IFileStorageService {
    public async Task ExportConversation(Conversation conversation, string filePath) {
        try {
            await File.WriteAllTextAsync(filePath, conversation.ToJson());
        } catch {
            throw;
        }
    }

    public async Task<Conversation> ImportConversation(string filePath) {
        if (!File.Exists(filePath)) {
            throw new FileNotFoundException("File not found", filePath);
        }

        try {
            var json = await File.ReadAllTextAsync(filePath);
            var conversation = json.FromJson();

            if (conversation == null) {
                throw new InvalidDataException("The file does not contain a valid conversation.");
            }

            return conversation;
        } catch {
            throw;
        }
    }

    public async Task<string> ReadTextFile(string filePath) {
        if(string.IsNullOrEmpty(filePath)) {
            throw new ArgumentException("filePath cannot be empty");
        }

        if(!File.Exists(filePath)) {
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
