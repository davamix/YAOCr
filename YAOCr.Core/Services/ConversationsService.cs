using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YAOCr.Core.Dtos;
using YAOCr.Core.Models;
using YAOCr.Core.Providers;

namespace YAOCr.Core.Services;

public interface IConversationsService
{
    Task<Message> SendMessage(List<Message> PreviuousMessages);
}

public class ConversationsService : IConversationsService {
    private readonly ILlmProvider _llmProvider;

    public ConversationsService(ILlmProvider llmProvider) {
        _llmProvider = llmProvider;
    }


    public async Task<Message> SendMessage(List<Message> PreviousMessages) {
        var dto = new SendMessageRequest {
            Model = "Gemma-3-4B-It",
            Messages = PreviousMessages.Select(m => new MessageDto {
                Role = m.Sender == SenderEnum.User ? "user" : "assistant",
                Content = m.Content
            }).ToList()
        };

        var response = await _llmProvider.SendMessage(dto);

        return new Message {
            Id = Guid.NewGuid().ToString(),
            Sender = SenderEnum.Assistant,
            Content = response,
            CreatedAt = DateTime.Now
        };
    }
}
