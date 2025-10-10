using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using System;
using YAOCr.Core.Models;

namespace YAOCr.ViewModels;

public partial class SettingsViewModel: ObservableObject {
    private readonly IConfiguration _configuration;

    [ObservableProperty]
    private Settings _settings = null;

    public SettingsViewModel(IConfiguration configuration) {
        _configuration = configuration;

        LoadConfiguration();
    }

    private void LoadConfiguration() {
        Settings = new Settings {
            Theme = _configuration["AppSettings:Theme"] ?? "Light",
            QdrantServerAddress = _configuration["AppSettings:Qdrant:Server"] ?? string.Empty,
            CompletionAddress = _configuration["AppSettings:LlamaCpp:CompletionAddress"] ?? string.Empty,
            ChatAddress = _configuration["AppSettings:LlamaCpp:ChatAddress"] ?? string.Empty,
            EmbeddingsAddress = _configuration["AppSettings:LlamaCpp:EmbeddingsAddress"] ?? string.Empty,
            ModelName = _configuration["AppSettings:LlamaCpp:ModelName"] ?? string.Empty
        };
    }

    [RelayCommand]
    private void SaveSettings() {
        _configuration["AppSettings:Theme"] = Settings.Theme;
        _configuration["AppSettings:Qdrant:Server"] = Settings.QdrantServerAddress;
        _configuration["AppSettings:LlamaCpp:ChatAddress"] = Settings.ChatAddress;
        _configuration["AppSettings:LlamaCpp:CompletionAddress"] = Settings.CompletionAddress;
        _configuration["AppSettings:LlamaCpp:EmbeddingsAddress"] = Settings.EmbeddingsAddress;
        _configuration["AppSettings:LlamaCpp:ModelName"] = Settings.ModelName;
    }
}
