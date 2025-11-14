using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YAOCr.Core.Dtos;
using YAOCr.Core.Models;
using YAOCr.Core.Providers;

namespace YAOCr.Core.Services;

public interface IConversationsService {
    IAsyncEnumerable<string> SendMessage(List<Message> conversationMessages);
    Task SaveConversation(Conversation conversation);
    Task SaveMessage(Message message, Guid conversationId);
    Task<IEnumerable<Conversation>> GetConversations();
    Task<IEnumerable<Message>> GetConversationMessages(Guid conversationId);
    Task DeleteConversation(Guid conversationId);
    Task DeleteMessage(Guid messageId);
}

public class ConversationsService : IConversationsService {
    private readonly ILlmService _llmService;
    private readonly ILlmProvider _llmProvider;
    private readonly IConversationProvider _conversationProvider;
    private readonly IConfiguration _configuration;
    private string _llmModel;
    private string _embeddingsModel;

    public ConversationsService(ILlmService llmService,
        ILlmProvider llmProvider,
        IConversationProvider conversationProvider,
        IConfiguration configuration) {
        _llmService = llmService;
        _llmProvider = llmProvider;
        _conversationProvider = conversationProvider;
        _configuration = configuration;

        Initialze();
    }

    private void Initialze() {
        _llmModel = _configuration["AppSettings:LlamaCpp:ModelName"];
        _embeddingsModel = _configuration["AppSettings:LlamaCpp:EmbeddingsModelName"];
    }

    public async IAsyncEnumerable<string> SendMessage(List<Message> conversationMessages) {
        var request = new SendMessageRequest {
            Stream = true,
            Model = _llmModel,
            Messages = conversationMessages.Select(m => new MessageDto {
                Role = m.Sender == SenderEnum.User ? "user" : "assistant",
                Content = m.Attachments.Any()
                    ? _llmService.CreateLlmMessage(m.Content, m.Attachments)
                    : m.Content
            }).ToList()
        };

        await foreach (var chunk in _llmProvider.SendMessage(request)) {
            yield return chunk;
        }
    }

    public async Task SaveConversation(Conversation conversation) {
        var request = new EmbeddingRequest {
            Content = conversation.Name,
            ModelName = _configuration["AppSettings:LlamaCpp:EmbeddingsModelName"],
            EncodingFormat = "float"
        };
        // Generate embeddings with conversation's Name
        var embeddings = await _llmProvider.GenerateEmbeddings(request);

        // Save conversation with embeddings
        await _conversationProvider.SaveConversation(conversation, embeddings);
    }

    public async Task SaveMessage(Message message, Guid conversationId) {
        var request = new EmbeddingRequest {
            Content = message.Content,
            ModelName = _embeddingsModel,
            EncodingFormat = "float"
        };

        // Generate embeddings with message's Content
        var embeddings = await _llmProvider.GenerateEmbeddings(request);

        // Save message with embeddings
        await _conversationProvider.SaveMessage(message, embeddings, conversationId);
    }

    public async Task<IEnumerable<Conversation>> GetConversations() {
        return await _conversationProvider.GetConversations();
    }

    public async Task<IEnumerable<Message>> GetConversationMessages(Guid conversationId) {
        return await _conversationProvider.GetMessages(conversationId);
    }

    public async Task DeleteConversation(Guid conversationId) {
        await _conversationProvider.DeleteConversation(conversationId);
    }

    public async Task DeleteMessage(Guid messageId) {
        await _conversationProvider.DeleteMessage(messageId);
    }
}
