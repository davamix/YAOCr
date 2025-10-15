using Microsoft.Extensions.Configuration;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YAOCr.Core.Models;

namespace YAOCr.Core.Providers;
public class QdrantProvider : IConversationProvider {
    private readonly IConfiguration _configuration;
    private QdrantClient _client;
    private string _collectionName;
    private ulong _embeddingsVectorSize;

    public QdrantProvider(IConfiguration configuration) {
        _configuration = configuration;

        Task.Run(Initialize).Wait();
        Task.Run(InitializeCollection).Wait();
    }

    private async Task Initialize() {
        _client = new QdrantClient(new Uri(_configuration["AppSettings:Qdrant:Server"]));
        _collectionName = _configuration["AppSettings:Qdrant:Collection"];
        _embeddingsVectorSize = ulong.Parse(_configuration["AppSettings:Qdrant:VectorSize"]);

        await Task.CompletedTask;
    }

    private async Task InitializeCollection() {
        try {
            if (await _client.CollectionExistsAsync(_collectionName)) return;

            await _client.CreateCollectionAsync(_collectionName, new VectorParams {
                Size = _embeddingsVectorSize,
                Distance = Distance.Cosine
            });
        } catch {
            throw;
        }
    }

    public Task<List<Conversation>> GetConversationsAsync() {
        return Task.FromResult(new List<Conversation>());
    }

    public async Task SaveConversation(Conversation conversation, List<float[]> embeddings) {
        try {
            var result = await _client.UpsertAsync(
                collectionName: _collectionName,
                points: new List<PointStruct> {
                    new() {
                        Id = conversation.Id,
                        Vectors = embeddings.SelectMany(x=>x).ToArray(),
                        Payload = {
                            ["Name"] = conversation.Name,
                            ["CreatedAt"] = conversation.CreatedAt.ToString(),
                            ["UpdatedAt"] = conversation.UpdatedAt.ToString()
                        }
                    }
                });
        } catch {
            throw;
        }
    }

    public async Task SaveMessage(Message message, List<float[]> embeddings, Guid conversationId) {
        try {
            var result = await _client.UpsertAsync(
                collectionName: _collectionName,
                points: new List<PointStruct> {
                    new() {
                        Id = message.Id,
                        Vectors = embeddings.SelectMany(x=>x).ToArray(),
                        Payload = {
                            ["ConversationId"] = conversationId.ToString(),
                            ["Content"] = message.Content,
                            ["Sender"] = message.Sender.ToString(),
                            ["CreatedAt"] = message.CreatedAt.ToString(),
                            ["UpdatedAt"] = message.UpdatedAt.ToString()
                        }
                    }
                });

            //return message;
        } catch {
            throw;
        }
    }
}
