using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.ObjectModel;
using System.Linq;
using YAOCr.Core.Dtos;
using YAOCr.Core.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YAOCr.Controls;

public sealed partial class ConversationMessageItem : UserControl {
    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.RegisterAttached(
            "Message",
            typeof(Message),
            typeof(ConversationMessageItem),
            new PropertyMetadata(null, OnMessageReceived)
        );

    public static readonly DependencyProperty ReloadMessageCommandProperty =
        DependencyProperty.RegisterAttached(
            "ReloadMessageCommand",
            typeof(RelayCommand<Message>),
            typeof(ConversationMessageItem),
            new PropertyMetadata(null)
        );

    public static readonly DependencyProperty EditMessageCommandProperty =
        DependencyProperty.RegisterAttached(
            "EditMessageCommand",
            typeof(RelayCommand<EditMessage>),
            typeof(ConversationMessageItem),
            new PropertyMetadata(null)
        );

    public Message Message {
        get { return (Message)GetValue(MessageProperty); }
        set { SetValue(MessageProperty, value); }
    }

    public RelayCommand<Message> ReloadMessageCommand {
        get { return (RelayCommand<Message>)GetValue(ReloadMessageCommandProperty); }
        set { SetValue(ReloadMessageCommandProperty, value); }
    }

    public RelayCommand<EditMessage> EditMessageCommand {
        get { return (RelayCommand<EditMessage>)GetValue(EditMessageCommandProperty); }
        set { SetValue(EditMessageCommandProperty, value); }
    }

    public ObservableCollection<string> FilesPath { get; private set; } = new();
    public EditMessage EditedMessage { get; set; } = new();

    private bool _isEditMode = false;

    public ConversationMessageItem() {
        InitializeComponent();
    }


    private void conversationMessageItemControl_Loaded(object sender, RoutedEventArgs e) {
        this.FadeInMessageItemStoryboard.Begin();
    }

    private static void OnMessageReceived(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        var message = e.NewValue as Message;

        if (message == null) return;

        ((ConversationMessageItem)d).EditedMessage.Message = message;
        ((ConversationMessageItem)d).EditedMessage.NewContent = message.Content;

        if (message.Attachments.Any()) {
            foreach (var att in message.Attachments) {
                ((ConversationMessageItem)d).FilesPath.Add(att.Path);
            }
        }
    }

    private void btn_PointerEntered(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }

    private void btn_PointerExited(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
    }

    private void gridMain_PointerEntered(object sender, PointerRoutedEventArgs e) {
        if (_isEditMode) return;

        if (Message?.Sender == SenderEnum.Assistant) {
            this.FadeInReloadButtonStoryboard.Begin();
        }

        this.FadeInEditButtonStoryboard.Begin();

    }

    private void gridMain_PointerExited(object sender, PointerRoutedEventArgs e) {
        if (_isEditMode) return;

        if (Message?.Sender == SenderEnum.Assistant) {
            this.FadeOutReloadButtonStoryboard.Begin();
        }

        this.FadeOutEditButtonStoryboard.Begin();
    }

    private void btnEdit_Click(object sender, RoutedEventArgs e) {
        mkdMessage.Visibility = Visibility.Collapsed;
        stackEditMessage.Visibility = Visibility.Visible;
        toolbar.Visibility = Visibility.Collapsed;

        _isEditMode = true;
    }

    private void btnSave_Click(object sender, RoutedEventArgs e) {
        stackEditMessage.Visibility = Visibility.Collapsed;
        mkdMessage.Visibility = Visibility.Visible;
        toolbar.Visibility = Visibility.Visible;

        _isEditMode = false;
    }
}
