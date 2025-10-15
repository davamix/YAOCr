using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YAOCr.Core.Models;

namespace YAOCr.Core.Extensions;
public static class ConversationToJson {
    public static string ToJson(this Conversation conversation) {
        string json = JsonSerializer.Serialize(conversation, new JsonSerializerOptions {
            WriteIndented = true,
            IncludeFields = true,
        });

        return json;
    }
}
