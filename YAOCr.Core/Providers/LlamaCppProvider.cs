using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using YAOCr.Core.Dtos;

namespace YAOCr.Core.Providers;

public class MessageApi {
    public string role { get; set; }
    public string content { get; set; }
}

public class MessageContent {
    public string model { get; set; }
    public List<MessageApi> messages { get; set; }
}


public class LlamaCppProvider : ILlmProvider {

    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient = new();
    private string _llmCompletionAddress;
    private string _llmChatAddress;



    public LlamaCppProvider(IConfiguration configuration) {
        _configuration = configuration;

        Initialize();
    }

    private void Initialize() {
        //_llmCompletionAddress = _configuration["AppSettings:LlamaCpp:CompletionAddress"];
        _llmChatAddress = _configuration["AppSettings:LlamaCpp:ChatAddress"];
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
