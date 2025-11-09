using Qdrant.Client.Grpc;
using System;
using YAOCr.Core.Models;

namespace YAOCr.Core.Mappers;
public static class ToConversationMapper {
    public static Conversation ToConversation(this RetrievedPoint point) {
        
        return new Conversation {
            Id = Guid.Parse(point.Id.Uuid),
            Name = point.GetPayloadStringValue("Name"),
            CreatedAt = point.GetPayloadDateTimeValue("CreatedAt"),
            UpdatedAt = point.GetPayloadDateTimeValue("UpdatedAt")
        };
    }
}
