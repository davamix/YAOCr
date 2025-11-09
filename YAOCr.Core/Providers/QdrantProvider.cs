using Microsoft.Extensions.Configuration;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using YAOCr.Core.Models;
using YAOCr.Core.Mappers;

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

            await _client.CreateCollectionAsync(
                _collectionName,
                new VectorParams {
                    Size = _embeddingsVectorSize,
                    Distance = Distance.Cosine
                },
                // Disable global index
                hnswConfig: new HnswConfigDiff {
                    PayloadM = 16,
                    M = 0
                }
            );

            // Set index by payload attribute "ConversationId"
            await _client.CreatePayloadIndexAsync(
                _collectionName,
                "ConversationId",
                schemaType: PayloadSchemaType.Keyword,
                indexParams: new PayloadIndexParams {
                    KeywordIndexParams = new KeywordIndexParams {
                        IsTenant = true
                    }
                });
        } catch {
            throw;
        }
    }

    public async Task<List<Conversation>> GetConversationsAsync() {
        var conversations = new List<Conversation>();

        try {
            var response = await _client.ScrollAsync(
                _collectionName,
                filter: Conditions.MatchKeyword("IsConversation", true.ToString()),
                //limit: 5, // Default limit = 10
                payloadSelector: true);

            //TODO: Map response to IEnumerable<Conversation>
            foreach (var result in response.Result) {
                var conversation = result.ToConversation();

                conversations.Add(conversation);
            }

            return conversations;
        } catch {
            throw;
        }
    }

    public async Task<List<Message>> GetMessages(Guid conversationId) {
        var messages = new List<Message>();

        try {
            var response = await _client.ScrollAsync(
                _collectionName,
                filter: Conditions.MatchKeyword("ConversationId", conversationId.ToString()),
                payloadSelector: true);

            foreach (var result in response.Result) {
                messages.Add(result.ToMessage());
            }

            return messages.OrderBy(x => x.CreatedAt).ToList();

        } catch {
            throw;
        }
    }

    public async Task SaveConversation(Conversation conversation, float[] embeddings) {
        try {
            var result = await _client.UpsertAsync(
                collectionName: _collectionName,
                points: new List<PointStruct> {
                    new() {
                        Id = conversation.Id,
                        Vectors = embeddings,
                        Payload = {
                            ["Name"] = conversation.Name,
                            ["IsConversation"] = true.ToString(),
                            ["CreatedAt"] = conversation.CreatedAt.ToString(),
                            ["UpdatedAt"] = conversation.UpdatedAt.ToString()
                        }
                    }
                });
        } catch {
            throw;
        }
    }

    public async Task SaveMessage(Message message, float[] embeddings, Guid conversationId) {
        try {
            var result = await _client.UpsertAsync(
                collectionName: _collectionName,
                points: new List<PointStruct> {
                    new() {
                        Id = message.Id,
                        Vectors = embeddings,
                        Payload = {
                            ["ConversationId"] = conversationId.ToString(),
                            ["Content"] = message.Content,
                            ["Sender"] = message.Sender.ToString(),
                            ["CreatedAt"] = message.CreatedAt.ToString(),
                            ["UpdatedAt"] = message.UpdatedAt.ToString(),
                            ["Attachments"] = new Value() {
                                StringValue = JsonSerializer.Serialize(message.Attachments)
                            }
                        }
                    }
                });
        } catch(Exception ex) {
            Debug.WriteLine($"[SaveMessage]: {ex.Message}");
            throw;
        }
    }
}
