using Qdrant.Client.Grpc;
using System;
using YAOCr.Core.Models;

namespace YAOCr.Core.Mappers;
public static class ToConversationMapper {
    public static Conversation ToConversation(this RetrievedPoint point) {
        
        return new Conversation {
            Id = Guid.Parse(point.Id.Uuid),
            Name = GetPayloadStringValue(point, "Name"),
            CreatedAt = GetPayloadDateTimeValue(point, "CreatedAt"),
            UpdatedAt = GetPayloadDateTimeValue(point, "UpdatedAt")
        };
    }

    private static DateTime GetPayloadDateTimeValue(RetrievedPoint point, string key) =>
        DateTime.Parse(GetPayloadStringValue(point, key));
    

    private static string GetPayloadStringValue(RetrievedPoint point, string key) {
        if(point.Payload.ContainsKey(key)) {
            return point.Payload[key].StringValue;
        }

        throw new Exception($"Key '{key}' not found in payload.");
    }
}
