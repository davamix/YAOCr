using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Linq;
using YAOCr.Core.Models;
using YAOCr.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YAOCr.Views;
public sealed partial class ConversationView : UserControl {

    public ConversationView() {
        this.InitializeComponent();

        this.DataContext = Ioc.Default.GetService<ConversationsViewModel>();

    }

    private void btn_PointerEntered(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }

    private void btn_PointerExited(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
    }

    // EventTriggerBehavior and InvokeCommandActions from XAML doesn't work.
    // Need to call command via event handler.
    private void lstConversations_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (!e.AddedItems.Any()) return;

        var context = this.DataContext as ConversationsViewModel;

        if (context == null) return;

        var item = e.AddedItems[0] as Conversation;

        context.LoadConversationMessagesCommand.Execute(item);
    }
}
