using System;
using System.Text.Json;
using YAOCr.Core.Models;

namespace YAOCr.Core.Extensions;

public static class ConversationToJson {
    private static readonly JsonSerializerOptions _writeOptions = new() {
        WriteIndented = true,
        IncludeFields = true,
    };

    private static readonly JsonSerializerOptions _readOptions = new() {
        IncludeFields = true,
    };

    public static string ToJson(this Conversation conversation) {
        string json = JsonSerializer.Serialize(conversation, _writeOptions);

        return json;
    }

    public static Conversation FromJson(this string json) {
        return JsonSerializer.Deserialize<Conversation>(json, _readOptions) ?? null;
    }
}
