using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using YAOCr.Core.Models;
using YAOCr.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YAOCr.Controls;

public sealed partial class ConversationItem : UserControl {

    public static readonly DependencyProperty ConversationProperty =
        DependencyProperty.RegisterAttached(
            "Conversation",
            typeof(Conversation),
            typeof(ConversationItem),
            new PropertyMetadata(null)
        );

    public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.RegisterAttached(
                "SaveCommand",
                typeof(RelayCommand<Conversation>),
                typeof(ConversationItem),
                new PropertyMetadata(null)
            );

    public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.RegisterAttached(
                "DeleteCommand",
                typeof(RelayCommand<Conversation>),
                typeof(ConversationItem),
                new PropertyMetadata(null)
            );

    public static readonly DependencyProperty ExportCommandProperty =
            DependencyProperty.RegisterAttached(
                "ExportCommand",
                typeof(RelayCommand<Conversation>),
                typeof(ConversationItem),
                new PropertyMetadata(null)
            );

    public Conversation Conversation {
        get { return (Conversation)GetValue(ConversationProperty); }
        set { SetValue(ConversationProperty, value); }
    }

    public RelayCommand<Conversation> SaveCommand {
        get { return (RelayCommand<Conversation>)GetValue(SaveCommandProperty); }
        set { SetValue(SaveCommandProperty, value); }
    }

    public RelayCommand<Conversation> DeleteCommand {
        get { return (RelayCommand<Conversation>)GetValue(DeleteCommandProperty); }
        set { SetValue(DeleteCommandProperty, value); }
    }

    public RelayCommand<Conversation> ExportCommand {
        get { return (RelayCommand<Conversation>)GetValue(ExportCommandProperty); }
        set { SetValue(ExportCommandProperty, value); }
    }

    public ConversationItem() {
        InitializeComponent();
    }

    private void btn_PointerEntered(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }

    private void btn_PointerExited(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
    }

    private void btnRenameConversation_Click(object sender, RoutedEventArgs e) {
        gridEditItem.Visibility = Visibility.Visible;
        gridReadItem.Visibility = Visibility.Collapsed;

        txtEditConversationName.Text = Conversation.Name;
    }

    private void btnSave_Click(object sender, RoutedEventArgs e) {
        SaveConversationName(Conversation.Name);
    }

    private void txtEditConversationName_KeyUp(object sender, KeyRoutedEventArgs e) {
        if(e.Key == Windows.System.VirtualKey.Enter) {
            Conversation.Name = txtEditConversationName.Text;
            SaveConversationName(Conversation.Name);
            SaveCommand?.Execute(Conversation);
        } else if (e.Key == Windows.System.VirtualKey.Escape) {
            SaveConversationName(txtConversationName.Text);
        }
    }

    private void gridReadItem_PointerEntered(object sender, PointerRoutedEventArgs e) {
        fadeInRenameButtonStoryboard.Begin();
    }

    private void gridReadItem_PointerExited(object sender, PointerRoutedEventArgs e) {
        fadeOutRenameButtonStoryboard.Begin();
    }

    private void SaveConversationName(string newValue) {
        gridReadItem.Visibility = Visibility.Visible;
        gridEditItem.Visibility = Visibility.Collapsed;

        txtConversationName.Text = Conversation.Name;
    }

    private async void btnExportConversation_Click(object sender, RoutedEventArgs e) {
        var button = sender as Button;

        if (button == null) return;

        var picker = new FileSavePicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);

        picker.FileTypeChoices.Add("JSON files", new[] { ".json" });
        picker.DefaultFileExtension = ".json";
        picker.SuggestedFileName = Conversation.Name;
        picker.CommitButtonText = "Save file";
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        picker.SuggestedFolder = "";

        var result = await picker.PickSaveFileAsync();

        if(result != null) {
            string savePath = result.Path;
            await File.WriteAllTextAsync(savePath, Conversation.ToJson());

        }
    }
}
