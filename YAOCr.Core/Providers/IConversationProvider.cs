using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YAOCr.Core.Models;

namespace YAOCr.Core.Providers;

public interface IConversationProvider {
    Task<List<Conversation>> GetConversations();
    Task<List<Message>> GetMessages(Guid conversationId);
    Task SaveMessage(Message message, float[] embeddings, Guid conversationId);
    Task SaveConversation(Conversation conversation, float[] embeddings);
    Task DeleteConversation(Guid conversationId);
}
