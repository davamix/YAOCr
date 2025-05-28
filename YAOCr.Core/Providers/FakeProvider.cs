using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAOCr.Core.Models;

namespace YAOCr.Core.Providers;

public interface IConversationProvider {
    Task<List<Conversation>> GetConversationsAsync();
}

public class FakeConversationProvider : IConversationProvider {
    private List<Conversation> _conversations = new();

    public FakeConversationProvider() {
        LoadConversations();
    }

    public Task<List<Conversation>> GetConversationsAsync() => Task.FromResult(_conversations);

    private void LoadConversations() {
        for (int i = 0; i < 5; i++) {
            _conversations.Add(GenerateConversation());
        }
    }

    private Conversation GenerateConversation() {
        return new Conversation {
            Id = Guid.NewGuid().ToString(),
            Name = "Conversation " + (_conversations.Count + 1),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Messages = new List<Message> {
                new Message {
                    Id = Guid.NewGuid().ToString(),
                    Sender = SenderEnum.User,
                    Content = "This is a sample message on conversation " + (_conversations.Count + 1),
                    CreatedAt = DateTime.Now
                },
                new Message {
                    Id = Guid.NewGuid().ToString(),
                    Sender = SenderEnum.Assistant,
                    Content = "This is a sample response on conversation " + (_conversations.Count + 1),
                    CreatedAt = DateTime.Now
                }
            }
        };
    }
}
