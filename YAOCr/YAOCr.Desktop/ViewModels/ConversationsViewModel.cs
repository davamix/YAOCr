using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using YAOCr.Core.Models;
using YAOCr.Core.Providers;
using YAOCr.Core.Services;
using YAOCr.Services;

namespace YAOCr.ViewModels;
public partial class ConversationsViewModel : ObservableObject {
    private readonly IConversationsService _conversationService;
    private readonly IConversationProvider _conversationProvider;
    private readonly ILlmService _llmService;
    private readonly IFileStorageService _fileStorageService;
    private readonly DialogService _dialogService;

    [ObservableProperty]
    private Conversation? _selectedConversation = null;

    [ObservableProperty]
    private bool _isLoading = false;

    public ObservableCollection<Conversation> Conversations { get; set; } = new();

    public ConversationsViewModel(IConversationsService conversationService,
        IConversationProvider conversationProvider,
        ILlmService llmService,
        IFileStorageService fileStorageService,
        DialogService dialogService) {
        _conversationService = conversationService;
        _conversationProvider = conversationProvider;
        _llmService = llmService;
        _fileStorageService = fileStorageService;
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
            Id = Guid.NewGuid(),
            Name = $"New [{DateTime.Now.ToShortDateString()}]",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };

        Conversations.Add(newConversation);

        SelectedConversation = newConversation;
    }

    [RelayCommand]
    private void ImportConversation(Conversation conversation) {
        if (conversation == null) return;

        if (Conversations.Any(c => c.Id == conversation.Id)) return;

        Conversations.Add(conversation);
    }

    [RelayCommand]
    private void SaveConversation(Conversation conversation) {
        //TODO: Save conversation name to DB
    }

    [RelayCommand]
    private void DeleteConvesation(Conversation conversation) {
        Conversations.Remove(conversation);

        //TODO: Delete conversation from DB
    }

    [RelayCommand]
    private async void SendMessage(PromptMessage promptMessage) {
        if (SelectedConversation == null) return;

        IsLoading = true;

        var filesContent = new List<(string Path, string Content)>();
        foreach(var f in promptMessage.FilePaths) {
            var content = await _fileStorageService.ReadTextFile(f);
            filesContent.Add((f, content));
        }

        var m = new Message(
            Id: Guid.NewGuid(),
            Content: promptMessage.Message,
            Sender: SenderEnum.User,
            CreatedAt: DateTime.Now,
            UpdatedAt: DateTime.Now,
            FilesContent: filesContent
        );

        try {
            // Save message then add to the list
            //await _conversationService.SaveMessage(m, SelectedConversation.Id);
            SelectedConversation.Messages.Add(m);

            var response = await _conversationService.SendMessage(SelectedConversation.Messages.ToList());
            //await _conversationService.SaveMessage(response, SelectedConversation.Id);
            SelectedConversation.Messages.Add(response);
        } catch {
            throw;
        } finally {
            IsLoading = false;
        }

    }

    [RelayCommand]
    private void OpenSettingsDialog() {
        _dialogService.OpenDialogSettings();
    }
}
