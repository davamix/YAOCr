using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using YAOCr.Core.Models;
using YAOCr.Core.Providers;
using YAOCr.Services;

namespace YAOCr.ViewModels;
public partial class ConversationsViewModel : ObservableObject {
    private readonly IConversationProvider _conversationProvider;
    private readonly DialogService _dialogService;

    public ObservableCollection<Conversation> Conversations { get; set; } = new();

    public ConversationsViewModel(IConversationProvider conversationProvider,
        DialogService dialogService) {
        _conversationProvider = conversationProvider;
        _dialogService = dialogService;
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

    [RelayCommand]
    private void OpenSettingsDialog() {
        _dialogService.OpenDialogSettings();
    }
}
