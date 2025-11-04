using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAOCr.Core.Dtos;

namespace YAOCr.Core.Providers;

public interface ILlmProvider {
    //Chat CreateChat();
    //IAsyncEnumerable<string> SendMessage(string message);
    IAsyncEnumerable<string> SendMessage(SendMessageRequest message);
    Task<float[]> GenerateEmbeddings(EmbeddingRequest request);
}
