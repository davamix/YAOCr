using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using YAOCr.Core.Dtos;

namespace YAOCr.Core.Providers;

public class LlamaCppProvider : ILlmProvider {

    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient = new();
    private string _llmCompletionAddress;
    private string _llmChatAddress;
    private string _llmEmbeddingsAddress;

    public LlamaCppProvider(IConfiguration configuration) {
        _configuration = configuration;

        Initialize();
    }

    private void Initialize() {
        //_llmCompletionAddress = _configuration["AppSettings:LlamaCpp:CompletionAddress"];
        _llmChatAddress = _configuration["AppSettings:LlamaCpp:ChatAddress"];
        _llmEmbeddingsAddress = _configuration["AppSettings:LlamaCpp:EmbeddingsAddress"];
    }

    public async Task<List<float[]>> GenerateEmbeddings(string text) {
        throw new NotImplementedException();

        var message = new StringContent(text);

        var response = await _httpClient.PostAsync(_llmEmbeddingsAddress, message);
        response.EnsureSuccessStatusCode();

        //TODO: Run an embeddings model with LlamaCpp



    }

    public async Task<string> SendMessage(SendMessageRequest message) {
        var messageContent = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_llmChatAddress, messageContent);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        var jsonResponse = JsonSerializer.Deserialize<JsonObject>(responseContent);

        // Go straight to the message content field
        return jsonResponse["choices"][0]["message"]["content"].GetValue<string>();

    }
}
