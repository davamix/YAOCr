using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAOCr.Core.Models;

namespace YAOCr.Themes.Selectors;

public class MessageStyleSelector : StyleSelector {
    protected override Style SelectStyleCore(object item, DependencyObject container) {
        if (container == null) return null;

        var resources = Application.Current.Resources;
        var message = item as Message;
        var isDarkTheme = false;

        var rootElement = container;
        //while (VisualTreeHelper.GetParent(rootElement) != null) {
        //    rootElement = VisualTreeHelper.GetParent(rootElement);
        //}
        
        
        if (rootElement is FrameworkElement element) {
            isDarkTheme = element.ActualTheme == ElementTheme.Dark;
        }

        if (message?.Sender == SenderEnum.User) {
            return resources[isDarkTheme ? "DarkListViewItemUserMessageStyle" : "LightListViewItemUserMessageStyle"] as Style;
        } else {
            return resources[isDarkTheme ? "DarkListViewItemAssistantMessageStyle" : "LightListViewItemAssistantMessageStyle"] as Style;
        }

        //if (message?.Sender == SenderEnum.User) {
        //    return resources["ThemedListViewItemUserMessageStyle"] as Style;
        //} else {
        //    return resources["ThemedListViewItemAssistantMessageStyle"] as Style;
        //}
    }
}
