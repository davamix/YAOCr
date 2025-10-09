using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using YAOCr.Core.Models;
using YAOCr.Core.Providers;
using YAOCr.Core.Services;
using YAOCr.Services;

namespace YAOCr.ViewModels;
public partial class ConversationsViewModel : ObservableObject {
    private readonly IConversationsService _conversationService;
    private readonly IConversationProvider _conversationProvider;
    private readonly DialogService _dialogService;

    [ObservableProperty]
    private Conversation? _selectedConversation = null;

    public ObservableCollection<Conversation> Conversations { get; set; } = new();

    public ConversationsViewModel(IConversationsService conversationService, IConversationProvider conversationProvider,
        DialogService dialogService) {
        _conversationService = conversationService;
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
    private void CreateConversation() {
        var newConversation = new Conversation {
            Id = Guid.NewGuid().ToString(),
            Name = $"New [{DateTime.Now.ToShortDateString()}]",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };

        Conversations.Add(newConversation);

        SelectedConversation = newConversation;
    }

    [RelayCommand]
    private void SaveConversation(Conversation conversation) {

    }

    [RelayCommand]
    private async void SendMessage(string message) {
        if (SelectedConversation == null) return;

        var m = new Message {
            Id = Guid.NewGuid().ToString(),
            Sender = SenderEnum.User,
            Content = message,
            CreatedAt = DateTime.Now
        };

        SelectedConversation.Messages.Add(m);

        var response = await _conversationService.SendMessage(SelectedConversation.Messages.ToList());

        SelectedConversation.Messages.Add(response);
    }

    [RelayCommand]
    private void OpenSettingsDialog() {
        _dialogService.OpenDialogSettings();
    }
}
