using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HuyaGPT.LMAPI;

public abstract class NetworkBasedModelProvider<T> : ILanguageModelProvider<T> where T : ChatResponseHandler
{
    public WebProxy? Proxy { get; }

    protected readonly HttpClient _client;
    
    protected NetworkBasedModelProvider(WebProxy? proxy)
    {
        Proxy = proxy;
        _client = new HttpClient();
        _client.Timeout = new TimeSpan(0,60,0);
    }
    
    protected HttpRequestMessage MakeRequest(string subPath, HttpMethod method, object? jsonParameters = null)
    {
        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Method = method;
        httpRequestMessage.RequestUri = new Uri(GetBasePath() + subPath);
        if (jsonParameters != null)
        {
            var serializeObject = JsonConvert.SerializeObject(jsonParameters);
            Console.WriteLine(serializeObject);
            httpRequestMessage.Content = new StringContent(serializeObject, Encoding.UTF8, "application/json");
        }
        return httpRequestMessage;
    }

    protected JObject ReadJObjectFromResponse(HttpResponseMessage responseMessage)
    {
        var readAsStringAsync = responseMessage.Content.ReadAsStringAsync();
        readAsStringAsync.Wait();
        return JObject.Parse(readAsStringAsync.Result);
    }

    protected abstract string GetBasePath();
    public abstract Task<T> Talk(string userMessage, Guid messageId, Guid parentMessageId, Guid? guid,
        bool stream);
    public abstract List<ModelDescription> GetModels();
    public abstract Conversation GetConversation(Guid conversationId);
    public abstract void DeleteConversation(Guid conversationId);
    public abstract void SetConversationTitle(string title, Guid conversationId);

    protected HttpResponseMessage SendRequest(HttpRequestMessage request)
    {
        return _client.Send(request);
    }
}