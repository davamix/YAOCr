using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Windows.Input;
using YAOCr.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YAOCr.Controls;

public class Prompt : TextBox {
    public static readonly DependencyProperty SendCommandProperty =
               DependencyProperty.Register(
                   "SendCommand",
                   typeof(ICommand),
                   typeof(Prompt),
                   new PropertyMetadata(null));
    
    public ICommand SendCommand {
        get { return (ICommand)GetValue(SendCommandProperty); }
        set { SetValue(SendCommandProperty, value); }
    }

    private ThemeService _themeService;

    public Prompt() {
        _themeService = Ioc.Default.GetService<ThemeService>()!;

        this.Loaded += Prompt_Loaded;
        _themeService.ThemeChanged += OnThemeChanged;
    }

    private void Prompt_Loaded(object sender, RoutedEventArgs e) {
        if (this.Style == null) {
            var resources = Application.Current.Resources;

            if (resources.ContainsKey("ThemedPromptStyle")) {
                this.Style = resources["ThemedPromptStyle"] as Style;
            }
        }
    }

    private void OnThemeChanged(object? sender, ElementTheme e) {
        var resources = Application.Current.Resources;

        if (e == ElementTheme.Light) {
            this.Style = resources["LightPromptStyle"] as Style;
        } else {
            this.Style = resources["DarkPromptStyle"] as Style;
        }

        ApplyTemplate();
    }

    protected override void OnApplyTemplate() {
        base.OnApplyTemplate();

        if (GetTemplateChild("SendButton") is Button sendButton) {
            sendButton.Command = this.SendCommand;
            sendButton.PointerEntered += btn_PointerEntered;
            sendButton.PointerExited += btn_PointerExited;
        }
    }

    private void btn_PointerEntered(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }

    private void btn_PointerExited(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
    }
}
