using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using YAOCr.Core.Models;
using YAOCr.Core.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YAOCr.Controls;
public sealed partial class CreateConversationSplitButton : UserControl {
    private IFileStorageService _fileStorageService;

    public static readonly DependencyProperty CreateNewConversationCommandProperty =
            DependencyProperty.RegisterAttached(
                "CreateNewConversationCommand",
                typeof(RelayCommand),
                typeof(CreateConversationSplitButton),
                new PropertyMetadata(null)
            );

    public static readonly DependencyProperty ImportConversationCommandProperty =
            DependencyProperty.RegisterAttached(
                "ImportConversationCommand",
                typeof(RelayCommand<Conversation>),
                typeof(CreateConversationSplitButton),
                new PropertyMetadata(null)
            );

    public RelayCommand CreateNewConversationCommand {
        get { return (RelayCommand)GetValue(CreateNewConversationCommandProperty); }
        set { SetValue(CreateNewConversationCommandProperty, value); }
    }

    public RelayCommand<Conversation> ImportConversationCommand {
        get { return (RelayCommand<Conversation>)GetValue(ImportConversationCommandProperty); }
        set { SetValue(ImportConversationCommandProperty, value); }
    }

    public CreateConversationSplitButton() {
        InitializeComponent();

        _fileStorageService = Ioc.Default.GetService<IFileStorageService>();
    }

    private void splitButtonMain_Click(SplitButton sender, SplitButtonClickEventArgs args) {
        CreateNewConversationCommand?.Execute(null);
    }

    private async void btnImportButton_Click(object sender, RoutedEventArgs args) {
        var button = sender as Button;

        if (button == null) return;

        var picker = new FileOpenPicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);

        picker.CommitButtonText = "Select file";
        picker.FileTypeFilter.Add(".json");
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        picker.ViewMode = PickerViewMode.List;

        var file = await picker.PickSingleFileAsync();

        if (file != null) {
            var conversation = await _fileStorageService.ImportConversation(file.Path);

            ImportConversationCommand?.Execute(conversation);
        }
    }

    private void btn_PointerEntered(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }

    private void btn_PointerExited(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
    }

    private void Flyout_Opened(object sender, object e) {
        var flyout = sender as Flyout;
        if (flyout == null) return;

        // Create a new style for FlyoutPresenter
        var baseStyle = flyout.FlyoutPresenterStyle ?? (Style)Application.Current.Resources["ThemedFlyoutPresenterStyle"];

        //var primaryButton = splitButtonMain.FindDescendant("PrimaryButton") as Button;

        // Clone the base style
        var newStyle = new Style { TargetType = typeof(FlyoutPresenter), BasedOn = baseStyle };
        newStyle.Setters.Add(new Setter(MinWidthProperty, splitButtonMain.ActualWidth));

        flyout.FlyoutPresenterStyle = newStyle;
    }

}

