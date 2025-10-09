using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAOCr.Core.Models;

namespace YAOCr.Core.Providers;

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
            Messages = new ObservableCollection<Message> {
                new Message {
                    Id = Guid.NewGuid().ToString(),
                    Sender = SenderEnum.User,
                    Content = "This is a sample message on conversation " + (_conversations.Count + 1),
                    CreatedAt = DateTime.Now
                },
                new Message {
                    Id = Guid.NewGuid().ToString(),
                    Sender = SenderEnum.Assistant,
                    Content = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam ac elit at neque efficitur porttitor ac sit amet tortor. 
                    Nunc finibus lacus a lectus elementum, a ullamcorper odio imperdiet. Ut dignissim, urna non rutrum ultricies, 
                    justo augue consectetur elit, id pharetra eros tellus quis nisl. Aliquam ultrices fringilla euismod. Etiam fringilla pellentesque 
                    odio vitae lacinia. Etiam condimentum massa augue, eget pharetra massa interdum sit amet.
                    Phasellus bibendum magna et sodales vehicula. Orci varius natoque penatibus et magnis dis parturient montes, 
                    nascetur ridiculus mus. 

```
public class Class{}
```

More text

* Item 1
* Item 2
                    ",
                    CreatedAt = DateTime.Now
                }
            }
        };
    }
}
