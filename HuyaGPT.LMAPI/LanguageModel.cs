using System.Net.Sockets;

namespace HuyaGPT.LMAPI;

public interface ILanguageModelProvider<T> where T : ChatResponseHandler
{
    public Task<T> Talk(string userMessage, Guid messageId, Guid parentMessageId, Guid? conversationId, bool stream);
    public List<ModelDescription> GetModels();
    public Conversation GetConversation(Guid conversationId);
    public void DeleteConversation(Guid conversationId);
    public void SetConversationTitle(string title, Guid conversationId);
}