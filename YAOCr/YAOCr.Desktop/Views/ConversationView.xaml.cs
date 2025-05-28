using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using YAOCr.Services;
using YAOCr.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YAOCr.Views;
public sealed partial class ConversationView : UserControl {

    private readonly ThemeService _themeService;
    public ConversationView() {
        this.InitializeComponent();

        this.DataContext = Ioc.Default.GetService<ConversationsViewModel>();

        _themeService = Ioc.Default.GetService<ThemeService>()!;
    }

    private void btnToDark_Click(object sender, RoutedEventArgs e) {
        var mw = ((App)Application.Current).MainWin;
        _themeService.ApplyTheme(mw, ElementTheme.Dark);
    }

    private void btnToLight_Click(object sender, RoutedEventArgs e) {
        var mw = ((App)Application.Current).MainWin;
        _themeService.ApplyTheme(mw, ElementTheme.Light);
    }

    private void btnConversation_PointerEntered(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }

    private void btnConversation_PointerExited(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
    }
}
