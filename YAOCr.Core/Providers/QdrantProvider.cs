using Microsoft.Extensions.Configuration;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YAOCr.Core.Models;

namespace YAOCr.Core.Providers;
public class QdrantProvider : IConversationProvider {
    private readonly IConfiguration _configuration;
    private QdrantClient _client;
    private string _collectionName;

    public QdrantProvider(IConfiguration configuration) {
        _configuration = configuration;

        Task.Run(Initialize).Wait();
        Task.Run(InitializeCollection).Wait();
    }

    public Task<List<Conversation>> GetConversationsAsync() {
        return Task.CompletedTask as Task<List<Conversation>>;
        //var conversations = _client.
    }

    private async Task Initialize() {
        _client = new QdrantClient(new Uri(_configuration["AppSettings:Qdrant:Server"]));
        _collectionName = _configuration["AppSettings:Qdrant:Collection"];
    }

    private async Task InitializeCollection() {
        if(await _client.CollectionExistsAsync(_collectionName)) return;

        await _client.CreateCollectionAsync(_collectionName, new VectorParams {
            Size = ulong.Parse(_configuration["AppSettings:Ollama:OutputVectorSize"]),
            Distance = Distance.Cosine
        });
    }
}
