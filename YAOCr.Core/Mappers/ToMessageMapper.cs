using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Text.Json;
using YAOCr.Core.Models;

namespace YAOCr.Core.Mappers;
public static class ToMessageMapper {
    public static Message ToMessage(this RetrievedPoint point) {

        return new Message(
            Id: Guid.Parse(point.Id.Uuid),
            Content: point.GetPayloadStringValue("Content"),
            Sender: point.GetPayloadStringValue("Sender") == "User" ? SenderEnum.User : SenderEnum.Assistant,
            CreatedAt: point.GetPayloadDateTimeValue("CreatedAt"),
            UpdatedAt: point.GetPayloadDateTimeValue("UpdatedAt"),
            Attachments: point.GetMessageFilesContent()
            );

    }

    private static List<MessageAttachment> GetMessageFilesContent(this RetrievedPoint point) {
        if (point.Payload.ContainsKey("Attachments")) {
            var content = point.Payload["Attachments"].StringValue;
            var attachments = JsonSerializer.Deserialize<List<MessageAttachment>>(content);
            
            return attachments ?? [];
        }

        throw new Exception($"Key 'Attachments' not found in payload.");
    }

}
