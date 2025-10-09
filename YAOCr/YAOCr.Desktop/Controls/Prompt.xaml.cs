using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Core;
using YAOCr.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YAOCr.Controls;

public class Prompt : TextBox {
    private Button? _sendButton;

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
        this.PreviewKeyDown += Prompt_PreviewKeyDown;
        _themeService.ThemeChanged += OnThemeChanged;
    }

    private void Prompt_PreviewKeyDown(object sender, KeyRoutedEventArgs e) {
        // https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.input.inputkeyboardsource.getkeystateforcurrentthread?view=windows-app-sdk-1.7
        // https://learn.microsoft.com/en-us/uwp/api/windows.ui.core.corevirtualkeystates?view=winrt-26100
        // https://github.com/microsoft/microsoft-ui-xaml/issues/6535#issuecomment-1057237421

        var isAltKeyPressed = (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift) 
            & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;

        if (e.Key == VirtualKey.Enter && isAltKeyPressed) {
            TextBox textBox = sender as TextBox;
            if (textBox != null) {
                var currentCursorPosition = textBox.SelectionStart;
                textBox.Text = textBox.Text.Insert(textBox.SelectionStart, Environment.NewLine);
                textBox.SelectionStart = currentCursorPosition + 1;
                e.Handled = true;
            }
        } else if (e.Key == VirtualKey.Enter) {
            if (this.SendCommand != null && this.SendCommand.CanExecute(this.Text)) {
                this.SendCommand.Execute(this.Text);
                this.Text = string.Empty;
            }
            e.Handled = true;
        }
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

        _sendButton = GetTemplateChild("SendButton") as Button;
        InitializeSendButton();
    }



    private void SendButton_Click(object sender, RoutedEventArgs e) {
        if (_sendButton != null) {
            _sendButton.CommandParameter = this.Text;
            this.Text = string.Empty;
        }
    }

    private void btn_PointerEntered(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }

    private void btn_PointerExited(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
    }

    private void InitializeSendButton() {
        if (_sendButton != null) {
            _sendButton.Command = this.SendCommand;
            _sendButton.Click += SendButton_Click;
            _sendButton.PointerEntered += btn_PointerEntered;
            _sendButton.PointerExited += btn_PointerExited;
        }
    }
}
