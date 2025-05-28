using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using YAOCr.Core.Models;
using CommunityToolkit.Mvvm.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YAOCr.Controls;

public sealed partial class ConversationItem : UserControl {

    public static readonly DependencyProperty ConversationProperty =
        DependencyProperty.RegisterAttached(
            "Conversation",
            typeof(Conversation),
            typeof(ConversationItem),
            new PropertyMetadata(null)
        );

    public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.RegisterAttached(
                "SaveCommand",
                typeof(RelayCommand<Conversation>),
                typeof(ConversationItem),
                new PropertyMetadata(null)
            );

    public Conversation Conversation {
        get { return (Conversation)GetValue(ConversationProperty); }
        set { SetValue(ConversationProperty, value); }
    }

    public RelayCommand<Conversation> SaveCommand {
        get { return (RelayCommand<Conversation>)GetValue(SaveCommandProperty); }
        set { SetValue(SaveCommandProperty, value); }
    }

    public ConversationItem() {
        InitializeComponent();
    }

    private void btnConversation_PointerEntered(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }

    private void btnConversation_PointerExited(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
    }

    private void btnRenameConversation_Click(object sender, RoutedEventArgs e) {
        gridEditItem.Visibility = Visibility.Visible;
        gridReadItem.Visibility = Visibility.Collapsed;
    }

    private void btnSave_Click(object sender, RoutedEventArgs e) {
        gridReadItem.Visibility = Visibility.Visible;
        gridEditItem.Visibility = Visibility.Collapsed;
    }
}
