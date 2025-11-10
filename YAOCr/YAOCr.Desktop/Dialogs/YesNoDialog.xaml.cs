using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using YAOCr.Services.Dialogs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YAOCr.Dialogs;
public sealed partial class YesNoDialog : UserControl {
    private DialogService _dialogService;
    private Action YesAction;
    private Action? NoAction;

    public static readonly DependencyProperty DialogTitleProperty =
        DependencyProperty.Register(
            nameof(DialogTitle), 
            typeof(string), 
            typeof(YesNoDialog), 
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DialogMessageProperty =
        DependencyProperty.Register(
            nameof(DialogMessage), 
            typeof(string), 
            typeof(YesNoDialog), 
            new PropertyMetadata(string.Empty));

    public string DialogTitle {
        get => (string)GetValue(DialogTitleProperty);
        set => SetValue(DialogTitleProperty, value);
    }

    public string DialogMessage {
        get => (string)GetValue(DialogMessageProperty);
        set => SetValue(DialogMessageProperty, value);
    }

    public YesNoDialog() {
        InitializeComponent();

        _dialogService = Ioc.Default.GetService<DialogService>()!;

        _dialogService.ShowYesNoDialogRequested += DialogService_ShowYesNoDialogRequested;
    }

    private void DialogService_ShowYesNoDialogRequested(object? sender, YesNoDialogArgs e) {
        this.DialogTitle = e.Title;
        this.DialogMessage = e.Message;
        this.YesAction = e.YesAction;
        this.NoAction = e.NoAction;
        this.Visibility = Visibility.Visible;
    }

    private void btnClose_Click(object sender, RoutedEventArgs e) {
        this.Visibility = Visibility.Collapsed;
    }

    private void btn_PointerEntered(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }

    private void btn_PointerExited(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
    }

    private void btnYes_Click(object sender, RoutedEventArgs e) {
        YesAction();

        this.Visibility = Visibility.Collapsed;
    }

    private void btnNo_Click(object sender, RoutedEventArgs e) {
        NoAction?.Invoke();

        this.Visibility = Visibility.Collapsed;
    }
}
