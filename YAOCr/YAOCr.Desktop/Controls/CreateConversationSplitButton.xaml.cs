using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YAOCr.Controls;
public sealed partial class CreateConversationSplitButton : UserControl {

    public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(RelayCommand),
                typeof(CreateConversationSplitButton),
                new PropertyMetadata(null)
            );

    public RelayCommand Command {
        get { return (RelayCommand)GetValue(ButtonCommandProperty); }
        set { SetValue(ButtonCommandProperty, value); }
    }

    public CreateConversationSplitButton() {
        InitializeComponent();
    }

    private void SplitButton_Click(SplitButton sender, SplitButtonClickEventArgs args) {
        Command?.Execute(null);
    }

    private void btn_PointerEntered(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }

    private void btn_PointerExited(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
    }

    private async void Flyout_Opened(object sender, object e) {
        var flyout = sender as Flyout;
        if (flyout == null) return;

        // Create a new style for FlyoutPresenter
        var baseStyle = flyout.FlyoutPresenterStyle ?? (Style)Application.Current.Resources["ThemedFlyoutPresenterStyle"];

        //var primaryButton = splitButtonMain.FindDescendant("PrimaryButton") as Button;
        
        // Clone the base style
        var newStyle = new Style { TargetType = typeof(FlyoutPresenter), BasedOn = baseStyle };
        newStyle.Setters.Add(new Setter(MinWidthProperty, splitButtonMain.ActualWidth));

        flyout.FlyoutPresenterStyle = newStyle;
    }
}

