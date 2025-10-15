using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YAOCr.Core.Models;

namespace YAOCr.Core.Providers;

public interface IConversationProvider {
    Task<List<Conversation>> GetConversationsAsync();
    Task SaveMessage(Message message, List<float[]> embeddings, Guid conversationId);
    Task SaveConversation(Conversation conversation, List<float[]> embeddings);
}
