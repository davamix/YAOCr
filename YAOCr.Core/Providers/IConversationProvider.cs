using System.Collections.Generic;
using System.Threading.Tasks;
using YAOCr.Core.Models;

namespace YAOCr.Core.Providers;

public interface IConversationProvider {
    Task<List<Conversation>> GetConversationsAsync();
}
