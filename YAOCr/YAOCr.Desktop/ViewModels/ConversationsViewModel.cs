using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
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
        GenerateFakeConversation();
        return;
        var conversations = await _conversationProvider.GetConversationsAsync();

        foreach (var conversation in conversations) {
            Conversations.Add(conversation);
        }
    }

    // TODO: To be removed
    private void GenerateFakeConversation() {
        var msg = """
            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse efficitur viverra metus, hendrerit sagittis elit pellentesque eu. Suspendisse non elit fermentum justo ultricies mollis. Cras quis interdum arcu. Nam varius urna non aliquet eleifend. Nunc eleifend nibh in tristique aliquet. Aliquam sed orci placerat, vulputate eros a, aliquet lectus. Duis aliquet tortor mi, vitae posuere ligula commodo cursus. Sed iaculis fringilla ante, et sollicitudin lorem molestie eu. Integer vehicula rhoncus neque quis fermentum. Vestibulum efficitur malesuada mi non eleifend. Suspendisse nec finibus justo. Ut arcu enim, venenatis ac ullamcorper in, ultricies vitae nulla. Nulla in mauris nec quam feugiat gravida. Nulla accumsan eu mi at pretium.
            """;
        var c = new Conversation {
            Id = Guid.NewGuid(),
            Name = "Fake Conversation",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        for (var x = 0; x < 10; x++) {
            c.Messages.Add(new Message(
                Id: Guid.NewGuid(),
                Content: msg,
                Sender: x % 2 == 0 ? SenderEnum.User : SenderEnum.Assistant,
                CreatedAt: DateTime.Now,
                UpdatedAt: DateTime.Now,
                FilesContent: []
            ));
        }

        Conversations.Add(c);
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

    private async Task<List<(string Path, string Content)>> ExtractFilesContent(IEnumerable<string> filesPath) {
        var filesContent = new List<(string Path, string Content)>();

        foreach (var f in filesPath) {
            var content = await _fileStorageService.ReadTextFile(f);
            filesContent.Add((f, content));
        }

        return filesContent;
    }

    private Message AddNewMessageToConversation(string messageContent, SenderEnum sender, List<(string Path, string Content)> filesContent) {
        var message = new Message(
            Id: Guid.NewGuid(),
            Content: messageContent,
            Sender: sender,
            CreatedAt: DateTime.Now,
            UpdatedAt: DateTime.Now,
            FilesContent: filesContent
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
