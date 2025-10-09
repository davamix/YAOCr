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
            OllamaServerAddress = _configuration["AppSettings:Ollama:Server"] ?? string.Empty,
            QdrantServerAddress = _configuration["AppSettings:Qdrant:Server"] ?? string.Empty,
            LlmModelName = _configuration["AppSettings:Ollama:LlmModel"] ?? string.Empty,
            EmbeddingsModelName = _configuration["AppSettings:Ollama:EmbeddingsModel"] ?? string.Empty,
            OutputVectorSize = _configuration["AppSettings:Ollama:OutputVectorSize"] ?? "768"
        };
    }

    [RelayCommand]
    private void SaveSettings() {
        _configuration["AppSettings:Theme"] = Settings.Theme;
        _configuration["AppSettings:Ollama:Server"] = Settings.OllamaServerAddress;
        _configuration["AppSettings:Qdrant:Server"] = Settings.QdrantServerAddress;
        _configuration["AppSettings:Ollama:LlmModel"] = Settings.LlmModelName;
        _configuration["AppSettings:Ollama:EmbeddingsModel"] = Settings.EmbeddingsModelName;
        _configuration["AppSettings:Ollama:OutputVectorSize"] = Settings.OutputVectorSize;
    }
}
