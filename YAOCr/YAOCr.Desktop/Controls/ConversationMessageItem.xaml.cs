using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using YAOCr.Core.Models;
using System.Collections.ObjectModel;
using Windows.Devices.Midi;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YAOCr.Controls;

public sealed partial class ConversationMessageItem : UserControl {
    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.RegisterAttached(
            "Message",
            typeof(Message),
            typeof(ConversationMessageItem),
            new PropertyMetadata(null, OnMessageReceived)
        );

    public Message Message {
        get { return (Message)GetValue(MessageProperty); }
        set { SetValue(MessageProperty, value); }
    }

    public ObservableCollection<string> FilesPath { get; private set; } = new();

    public ConversationMessageItem() {
        InitializeComponent();
    }


    private void conversationMessageItemControl_Loaded(object sender, RoutedEventArgs e) {
        this.ConversationMessageItemFadeInStoryboard.Begin();
    }

    private static void OnMessageReceived(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        var message = e.NewValue as Message;

        if (message != null) {
            foreach (var file in message.FilesContent) {
                ((ConversationMessageItem)d).FilesPath.Add(file.Path);
            }
        }
    }
}
