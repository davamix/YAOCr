using System.Collections.Generic;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
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
}
