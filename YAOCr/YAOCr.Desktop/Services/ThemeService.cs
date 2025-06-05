using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace YAOCr.Services;

public class ThemeService {
    public event EventHandler<ElementTheme> ThemeChanged;

    public void ApplyTheme(Window window, ElementTheme theme) {
        if (window.Content is FrameworkElement rootElement) {
            rootElement.RequestedTheme = theme;

            ThemeChanged?.Invoke(this, theme);

            //window.Activate();
        }
    }
}
