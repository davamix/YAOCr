using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using YAOCr.Core.Models;
using System.Collections.ObjectModel;

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

    public Message Message {
        get { return (Message)GetValue(MessageProperty); }
        set { SetValue(MessageProperty, value); }
    }

    public ObservableCollection<string> FilesPath { get; private set; } = new();

    public ConversationMessageItem() {
        InitializeComponent();
    }


    private void conversationMessageItemControl_Loaded(object sender, RoutedEventArgs e) {
        this.ConversationMessageItemFadeInStoryboard.Begin();
    }

    private static void OnMessageReceived(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        var message = e.NewValue as Message;

        if ((message != null) && (message.FilesContent.Any())) {
            foreach (var file in message.FilesContent) {
                ((ConversationMessageItem)d).FilesPath.Add(file.Path);
            }
        }
    }
}
