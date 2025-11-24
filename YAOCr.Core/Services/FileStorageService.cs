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
}
