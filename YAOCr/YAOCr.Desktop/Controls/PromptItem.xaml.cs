using CommunityToolkit.WinUI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using YAOCr.Core.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YAOCr.Controls;

public sealed partial class PromptItem : UserControl
{
    public PromptItem()
    {
        InitializeComponent();
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e) {
        var prompt = this.FindAscendant<Prompt>();
        var item = this.DataContext as FileParserContent;

        if((prompt != null) && (item != null)){
            prompt.FileParsersContent.Remove(item);
        }
    }

    private void btn_PointerEntered(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }

    private void btn_PointerExited(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
    }

}
