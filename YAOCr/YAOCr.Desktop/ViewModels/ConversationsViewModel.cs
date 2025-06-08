using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using YAOCr.Core.Models;
using YAOCr.Core.Providers;

namespace YAOCr.ViewModels;
public partial class ConversationsViewModel : ObservableObject {
    private readonly IConversationProvider _conversationProvider;

    public ObservableCollection<Conversation> Conversations { get; set; } = new();

    public ConversationsViewModel(IConversationProvider conversationProvider) {
        _conversationProvider = conversationProvider;

        InitializeConversations();
    }

    private async void InitializeConversations() {
        var conversations = await _conversationProvider.GetConversationsAsync();

        foreach (var conversation in conversations) {
            Conversations.Add(conversation);
        }
    }

    [RelayCommand]
    private void SaveConversation(Conversation conversation) {

    }

    [RelayCommand]
    private void Send() {
        Debug.WriteLine("Send command executed");
    }
}
