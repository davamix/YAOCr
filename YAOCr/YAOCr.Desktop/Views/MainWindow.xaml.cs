using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using YAOCr.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YAOCr.Views;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window {
    private readonly ThemeService _themeService;
    public ElementTheme RequestedTheme { get; private set; } = ElementTheme.Light;

    public MainWindow() {
        this.InitializeComponent();

        _themeService = Ioc.Default.GetService<ThemeService>()!;

        //_themeService.ThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged(object? sender, ElementTheme e) {
        RequestedTheme = e;

        if (this.Content is FrameworkElement rootElement) {
            rootElement.RequestedTheme = e;

            var listViews = FindVisualChildren<ListView>(rootElement);
            foreach (var listView in listViews) {
                var containerStyleSelector = listView.ItemContainerStyleSelector;

                listView.ItemContainerStyleSelector = null;
                listView.ItemContainerStyleSelector = containerStyleSelector;
            }
        }
    }

    private IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject {
        var count = VisualTreeHelper.GetChildrenCount(parent);
        for (var i = 0; i < count; i++) {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T childType) {
                yield return childType;
            }

            foreach (var grandChild in FindVisualChildren<T>(child)) {
                yield return grandChild;
            }
        }
    }

    // Remember to unsubscribe when the window is closed
    private void Window_Closed(object sender, WindowEventArgs args) {
        _themeService.ThemeChanged -= OnThemeChanged;
    }
}
