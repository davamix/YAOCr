using Microsoft.Extensions.Configuration;
using Microsoft.UI.Composition;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

    public async Task<float[]> GenerateEmbeddings(EmbeddingRequest request) {
        var messageContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        try {
            var jsonResponse = await RequestPost(_llmEmbeddingsAddress, messageContent);
            var embeddings = jsonResponse["data"][0]["embedding"];

            return JsonSerializer.Deserialize<float[]>(embeddings);
        } catch (Exception ex) {
            Debug.WriteLine(ex.Message);
            throw;
        }
    }

    public async IAsyncEnumerable<string> SendMessage(SendMessageRequest message) {
        var messageContent = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");

        await foreach (var chunk in RequestStreamPost(_llmChatAddress, messageContent)) {
            yield return chunk;
        }
    }

    private async Task<JsonObject> RequestPost(string address, StringContent content) {
        var response = await _httpClient.PostAsync(address, content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var returnValue = JsonSerializer.Deserialize<JsonObject>(responseContent);

        return returnValue;
    }

    private async IAsyncEnumerable<string> RequestStreamPost(string address, StringContent content) {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, address);
        requestMessage.Content = content;

        var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

        response.EnsureSuccessStatusCode();

        using (var stream = await response.Content.ReadAsStreamAsync()) {
            using (var reader = new StreamReader(stream)) {
                await foreach (var chunk in ProcessResponseStream(reader)) {
                    yield return chunk;
                }
            }
        }
    }

    private async IAsyncEnumerable<string> ProcessResponseStream(StreamReader reader) {
        while (!reader.EndOfStream) {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(line)) {
                continue;
            }

            var jsonData = line;
            if (line.StartsWith("data: ")) {
                jsonData = line.Substring("data: ".Length);
            }

            var doc = new JsonObject();
            try {
                doc = JsonSerializer.Deserialize<JsonObject>(jsonData);
            }catch {
                // line -> "data: [DONE]"
                continue;
            }

            var isFinished = doc["choices"][0]["finish_reason"] == null
            ? false
            : doc["choices"][0]["finish_reason"].GetValue<string>() == "stop";

            if (isFinished) {
                break;
            }

            var deltaContent = doc["choices"][0]["delta"]["content"];
            if (deltaContent == null) {
                continue;
            }

            yield return deltaContent.GetValue<string>();
        }
    }
}
