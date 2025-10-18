using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using YAOCr.Core.Dtos;
using YAOCr.Core.Models;
using YAOCr.Core.Providers;

namespace YAOCr.Core.Services;

public interface IConversationsService
{
    Task<Message> SendMessage(List<Message> PreviuousMessages);
    [Experimental("NotImplemented")]
    Task SaveConversation(Conversation conversation);
    [Experimental("NotImplemented")]
    Task SaveMessage(Message message, Guid conversationId);
}

public class ConversationsService : IConversationsService {
    private readonly ILlmService _llmService;
    private readonly ILlmProvider _llmProvider;
    private readonly IConversationProvider _conversationProvider;

    public ConversationsService(ILlmService llmService, 
        ILlmProvider llmProvider, 
        IConversationProvider conversationProvider) {
        _llmService = llmService;
        _llmProvider = llmProvider;
        _conversationProvider = conversationProvider;
    }


    public async Task<Message> SendMessage(List<Message> previousMessages) {

        var dto = new SendMessageRequest {
            Model = "Gemma-3-4B-It",
            Messages = previousMessages.Select(m => new MessageDto {
                Role = m.Sender == SenderEnum.User ? "user" : "assistant",
                Content = _llmService.CreateLlmMessage(m.Content, m.FilesContent)
            }).ToList()
        };

        var response = await _llmProvider.SendMessage(dto);

        return new Message(
            Id: Guid.NewGuid(),
            Content: response,
            Sender: SenderEnum.Assistant,
            CreatedAt: DateTime.Now,
            UpdatedAt: DateTime.Now,
            FilesContent: []
        );
    }

    public async Task SaveConversation(Conversation conversation) {
        throw new NotImplementedException();
        // Generate embeddings with conversation's Name
        var embeddings = await _llmProvider.GenerateEmbeddings(conversation.Name);

        // Save conversation with embeddings
        await _conversationProvider.SaveConversation(conversation, embeddings);
    }

    public async Task SaveMessage(Message message, Guid conversationId) {
        throw new NotImplementedException();
        // Generate embeddings with message's Content
        var embeddings = await _llmProvider.GenerateEmbeddings(message.Content);

        // Save message with embeddings
        await _conversationProvider.SaveMessage(message, embeddings, conversationId);
    }
}
