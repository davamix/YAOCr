using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using YAOCr.Core.Models;
using YAOCr.Core.Plugins;
using YAOCr.Plugins.Parsers;
using YAOCr.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YAOCr.Controls;

public class Prompt : TextBox {
    private ThemeService _themeService;
    private Button? _sendButton;
    private ListView? _listViewFilePaths;

    private readonly Dictionary<string, string[]> _contentTypesAllowed = new();

    private readonly Dictionary<string, string[]> _fileTypesAllowed = new();

    public ObservableCollection<FileParserContent> FileParsersContent { get; private set; } = new();

    public static readonly DependencyProperty SendCommandProperty =
               DependencyProperty.Register(
                   "SendCommand",
                   typeof(ICommand),
                   typeof(Prompt),
                   new PropertyMetadata(null));

    public ICommand SendCommand {
        get { return (ICommand)GetValue(SendCommandProperty); }
        set { SetValue(SendCommandProperty, value); }
    }

    public Prompt() {
        _themeService = Ioc.Default.GetService<ThemeService>()!;

        this.Loaded += Prompt_Loaded;
        this.PreviewKeyDown += Prompt_PreviewKeyDown;
        _themeService.ThemeChanged += OnThemeChanged;

        this.AllowDrop = true;
        this.DragOver += Prompt_DragOver;
        this.Drop += Prompt_Drop;

        FileParsersContent.CollectionChanged += FileParsersContent_CollectionChanged;

        LoadAllowedFileTypes();
    }

    private void LoadAllowedFileTypes() {
        foreach (var plugin in PluginsLoader.GetPlugins().Values) {
            var p = plugin as IFileParser;

            if (p == null) continue;

            _contentTypesAllowed.Add(p.Id, p.ContentTypes.ToArray());
            _fileTypesAllowed.Add(p.Id, p.Extensions.ToArray());
        }
    }

    private void FileParsersContent_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add) {
            _listViewFilePaths.Visibility = Visibility.Visible;
        }

        if (e.Action == NotifyCollectionChangedAction.Remove) {
            if (FileParsersContent.Count == 0) {
                _listViewFilePaths.Visibility = Visibility.Collapsed;
            }
        }
    }

    private void Prompt_PreviewKeyDown(object sender, KeyRoutedEventArgs e) {
        // https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.input.inputkeyboardsource.getkeystateforcurrentthread?view=windows-app-sdk-1.7
        // https://learn.microsoft.com/en-us/uwp/api/windows.ui.core.corevirtualkeystates?view=winrt-26100
        // https://github.com/microsoft/microsoft-ui-xaml/issues/6535#issuecomment-1057237421

        var isAltKeyPressed = (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift)
            & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;

        if (e.Key == VirtualKey.Enter && isAltKeyPressed) {
            TextBox textBox = sender as TextBox;
            if (textBox != null) {
                var currentCursorPosition = textBox.SelectionStart;
                textBox.Text = textBox.Text.Insert(textBox.SelectionStart, Environment.NewLine);
                textBox.SelectionStart = currentCursorPosition + 1;
                e.Handled = true;
            }
        } else if (e.Key == VirtualKey.Enter) {
            SendMessage();

            e.Handled = true;
        }
    }

    private void Prompt_Loaded(object sender, RoutedEventArgs e) {
        if (this.Style == null) {
            var resources = Application.Current.Resources;

            if (resources.ContainsKey("ThemedPromptStyle")) {
                this.Style = resources["ThemedPromptStyle"] as Style;
            }
        }
    }

    private void Prompt_DragOver(object sender, DragEventArgs e) {
        e.AcceptedOperation = DataPackageOperation.Copy;
    }

    private async void Prompt_Drop(object sender, DragEventArgs e) {
        if (!e.DataView.Contains(StandardDataFormats.StorageItems)) return;

        var storageItems = await e.DataView.GetStorageItemsAsync();

        foreach (var item in storageItems) {
            if (!item.IsOfType(StorageItemTypes.File)) continue;

            var storageFile = item as StorageFile;

            // Try to get parser by content type first
            var contentTypeParserId = _contentTypesAllowed.FirstOrDefault(x => x.Value.Contains(storageFile.ContentType)).Key;
            if (contentTypeParserId != null) {
                FileParsersContent.Add(new FileParserContent {
                    ParserId = contentTypeParserId,
                    FilePath = item.Path
                });

                continue;
            }

            // Try to get parser by file extension
            var fileTypeParserId = _fileTypesAllowed.FirstOrDefault(x => x.Value.Contains(storageFile.FileType)).Key;
            if (fileTypeParserId != null) {
                FileParsersContent.Add(new FileParserContent {
                    ParserId = fileTypeParserId,
                    FilePath = item.Path
                });
            }
        }
    }

    private void OnThemeChanged(object? sender, ElementTheme e) {
        var resources = Application.Current.Resources;

        if (e == ElementTheme.Light) {
            this.Style = resources["LightPromptStyle"] as Style;
        } else {
            this.Style = resources["DarkPromptStyle"] as Style;
        }

        ApplyTemplate();
    }

    private void SendButton_Click(object sender, RoutedEventArgs e) {
        if (_sendButton != null) {
            SendMessage();
        }
    }

    private void btn_PointerEntered(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }

    private void btn_PointerExited(object sender, PointerRoutedEventArgs e) {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
    }

    protected override void OnApplyTemplate() {
        base.OnApplyTemplate();

        _sendButton = GetTemplateChild("SendButton") as Button;
        InitializeSendButton();

        _listViewFilePaths = GetTemplateChild("ListViewFilePaths") as ListView;
        InitializeListViewFilePaths();
    }

    private void InitializeSendButton() {
        if (_sendButton != null) {
            _sendButton.Click += SendButton_Click;
            _sendButton.PointerEntered += btn_PointerEntered;
            _sendButton.PointerExited += btn_PointerExited;
        }
    }

    private void InitializeListViewFilePaths() {
        if (_listViewFilePaths != null) {
            _listViewFilePaths.ItemsSource = FileParsersContent;
            if (FileParsersContent.Any()) {
                _listViewFilePaths.Visibility = Visibility.Visible;
            }
        }
    }

    private void SendMessage() {
        var promptMessage = new PromptMessage(
            Message: this.Text,
            FileParsersContent: FileParsersContent.ToList());

        if (this.SendCommand != null && this.SendCommand.CanExecute(promptMessage)) {
            this.SendCommand.Execute(promptMessage);

            this.Text = string.Empty;
            FileParsersContent.Clear();
            _listViewFilePaths.Visibility = Visibility.Collapsed;
        }
    }
}
