using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using YAOCr.Services;
using YAOCr.Services.Dialogs;
using YAOCr.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YAOCr.Views;

public sealed partial class SettingsView : UserControl
{
    private DialogService _dialogService;
    private ThemeService _themeService;

    public SettingsView()
    {
        InitializeComponent();

        _dialogService = Ioc.Default.GetService<DialogService>()!;
        _themeService = Ioc.Default.GetService<ThemeService>()!;

        _dialogService.ShowDialogSettingsRequested += DialogService_ShowDialogSettingsRequested;

        this.DataContext = Ioc.Default.GetService<SettingsViewModel>();
    }

    private void DialogService_ShowDialogSettingsRequested(object? sender, EventArgs e) {
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

    private void cmbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        var mw = ((App)Application.Current).MainWin;

        var selectedTheme = cmbTheme.SelectedItem.ToString() == "Light" ? ElementTheme.Light : ElementTheme.Dark;

        if (mw != null) {
            _themeService.ApplyTheme(mw, selectedTheme);
        }

    }
}
