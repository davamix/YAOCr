using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAOCr.Core.Models;
using YAOCr.Core.Providers;
using YAOCr.Core.Services;
using YAOCr.Services.Dialogs;

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
    private bool _isWaitingForResponse = false;

    [ObservableProperty]
    private string _statusMessage = string.Empty;


    private StringBuilder _assistantMessageStream = new();
    public string AssistantMessage {
        get => _assistantMessageStream.ToString();
        set {
            _assistantMessageStream.Append(value);
            OnPropertyChanged();
        }
    }

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
        var conversations = await _conversationService.GetConversations();// _conversationProvider.GetConversationsAsync();

        foreach (var conversation in conversations) {
            Conversations.Add(conversation);
        }
    }

    [RelayCommand]
    private async void CreateConversation() {
        var newConversation = new Conversation {
            Id = Guid.NewGuid(),
            Name = $"New [{DateTime.Now.ToShortDateString()}]",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };

        try {
            SetStatusMessage("Creating new conversation...");

            await _conversationService.SaveConversation(newConversation);

            Conversations.Add(newConversation);

            SelectedConversation = newConversation;
        } catch {
            SetStatusMessage("Failed to create new conversation.");
            throw;
        } finally {
            SetStatusMessage(string.Empty);
        }
    }

    [RelayCommand]
    private void ImportConversation(Conversation conversation) {
        if (conversation == null) return;

        if (Conversations.Any(c => c.Id == conversation.Id)) return;

        Conversations.Add(conversation);
    }

    [RelayCommand]
    private async Task SaveConversation(Conversation conversation) {
        try {
            SetStatusMessage("Saving conversation...");

            conversation.UpdatedAt = DateTime.Now;

            await _conversationService.SaveConversation(conversation);
        } catch {
            SetStatusMessage("Failed to save conversation.");
            throw;
        } finally {
            SetStatusMessage(string.Empty);
        }
    }

    [RelayCommand]
    private void DeleteConversation(Conversation conversation) {
        var message = DialogMessages.ConfirmDeleteConversationMessage(conversation.Name);

        _dialogService.OpenYesNoDialog(new YesNoDialogArgs {
            Title = message.Title,
            Message = message.Message,
            YesAction = async ()=> await DeleteConversationConfirmed(conversation)
        });
    }

    private async Task DeleteConversationConfirmed(Conversation conversation) {
        try {
            SetStatusMessage("Deleting conversation...");

            await _conversationService.DeleteConversation(conversation.Id);

            Conversations.Remove(conversation);
        } catch {
            throw;
        } finally {
            SetStatusMessage(string.Empty);
        }
    }

    [RelayCommand]
    private async Task LoadConversationMessages(Conversation conversation) {
        if (SelectedConversation.Messages.Any()) return;

        var messages = await _conversationService.GetConversationMessages(conversation.Id);

        foreach (var msg in messages) {
            SelectedConversation.Messages.Add(msg);
        }
    }

    [RelayCommand]
    private async void SendMessage(PromptMessage promptMessage) {
        if (SelectedConversation == null) return;

        try {
            await ProcesssUserMessage(promptMessage);
            await ProcessAssistantMessage();
        } catch {
            throw;
        }
    }

    private async Task ProcesssUserMessage(PromptMessage promptMessage) {
        try {
            StartWaitingForResponse();

            SetStatusMessage("Processing user message...");
            var filesContent = await ExtractFilesContent(promptMessage.FilePaths);

            // Add user message to the conversation
            var message = AddNewMessageToConversation(promptMessage.Message, SenderEnum.User, filesContent);

            // Save message then add to the list
            await _conversationService.SaveMessage(message, SelectedConversation.Id);
        } catch {
            throw;
        } finally {
            SetStatusMessage(string.Empty);
            StopWaitingForResponse();
        }
    }

    private async Task ProcessAssistantMessage() {
        _ = DispatcherQueue.GetForCurrentThread().TryEnqueue(async () => {
            StartWaitingForResponse();
            SetStatusMessage("Waiting for assistant response...");

            try {
                await ProcessAssistantMessageResponse();

                // Add assistant message to the conversation
                var message = AddNewMessageToConversation(AssistantMessage, SenderEnum.Assistant, []);

                // Save assistant message to db
                await _conversationService.SaveMessage(message, SelectedConversation.Id);

                _assistantMessageStream.Clear();
                RefreshProperty(AssistantMessage);
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            } finally {
                StopWaitingForResponse();
                SetStatusMessage(string.Empty);
            }
        });
    }

    private async Task ProcessAssistantMessageResponse() {
        await foreach (var response in _conversationService.SendMessage(SelectedConversation?.Messages.ToList())) {
            if (IsWaitingForResponse) {
                SetStatusMessage("Receiving assistant response...");
                StopWaitingForResponse();
            }

            await Task.Delay(1);

            AssistantMessage = response;
        }
    }

    [RelayCommand]
    private void OpenSettingsDialog() {
        _dialogService.OpenDialogSettings();
    }

    private async Task<List<MessageAttachment>> ExtractFilesContent(IEnumerable<string> filesPath) {
        var filesContent = new List<MessageAttachment>();

        foreach (var f in filesPath) {
            var content = await _fileStorageService.ReadTextFile(f);
            var attachment = new MessageAttachment {
                Path = f,
                Content = content
            };

            filesContent.Add(attachment);
        }

        return filesContent;
    }

    private Message AddNewMessageToConversation(string messageContent, SenderEnum sender, List<MessageAttachment> attachments) {
        var message = new Message(
            Id: Guid.NewGuid(),
            Content: messageContent,
            Sender: sender,
            CreatedAt: DateTime.Now,
            UpdatedAt: DateTime.Now,
            Attachments: attachments
        );

        SelectedConversation.Messages.Add(message);

        return message;
    }

    private void StartWaitingForResponse() {
        IsWaitingForResponse = true;
        RefreshProperty(nameof(IsWaitingForResponse));
    }

    private void StopWaitingForResponse() {
        IsWaitingForResponse = false;
        RefreshProperty(nameof(IsWaitingForResponse));
    }

    private void SetStatusMessage(string message) {
        _ = DispatcherQueue.GetForCurrentThread().TryEnqueue(() => {
            StatusMessage = message;
            RefreshProperty(nameof(StatusMessage));
        });
    }

    private void RefreshProperty(string propertyName) {
        OnPropertyChanged(propertyName);
    }
}
