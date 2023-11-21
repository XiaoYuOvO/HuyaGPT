using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HuyaGPT.LMAPI.pandora;

public class PandoraGPTModelProvider : OnlineAuthLanguageModelProvider<PandoraChatResponseHandler>
{
    private readonly string _pandoraLaunchCmd;
    private readonly int _pandoraServicePort;
    private string? _pandoraServicePath = "http://127.0.0.1:6666";

    protected override string GetBasePath()
    {
        return _pandoraServicePath ?? throw new InvalidOperationException("Provider has not login");
    }

    protected override void DoLogin(string username, string password)
    {
        var process = new Process();
        process.StartInfo.FileName = _pandoraLaunchCmd;

        if (Proxy != null && Proxy.Address != null)
        {
            process.StartInfo.Arguments = $"-p {Proxy.Address} ";
        }
        
        _pandoraServicePath = $"127.0.0.1:{_pandoraServicePort}";
        process.StartInfo.Arguments += $"-s {_pandoraServicePath}";
        _pandoraServicePath = $"http://127.0.0.1:{_pandoraServicePort}";
        process.StartInfo.CreateNoWindow = false;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.EnvironmentVariables["PPID"] = Process.GetCurrentProcess().Id.ToString();
        process.StartInfo.EnvironmentVariables["OPENAI_EMAIL"] = username;
        process.StartInfo.EnvironmentVariables["OPENAI_PASSWORD"] = password;
        process.StartInfo.EnvironmentVariables["OPENAI_MFA_CODE"] = " ";
        AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
        {
            process.Kill(true);
        };
        process.StartInfo.RedirectStandardError = true;
        process.Start();
        string? outLine = "";
        while (outLine == null || !outLine.Contains("Serving on"))
        {
            outLine = process.StandardError.ReadLine();
            Task.Delay(100).Wait();
        }
    }

    public override async Task<PandoraChatResponseHandler> Talk(string userMessage, Guid messageId, Guid parentMessageId, Guid? conversationId, bool stream)
    {
        return await Task.Run(() =>
        {
            object jsonParameters = conversationId != null
                ? new
                {
                    prompt = userMessage,
                    model = "text-davinci-002-render-sha",
                    message_id = messageId,
                    parent_message_id = parentMessageId,
                    conversation_id = conversationId,
                    stream,
                }
                : new
                {
                    prompt = userMessage,
                    model = "text-davinci-002-render-sha",
                    message_id = messageId,
                    parent_message_id = parentMessageId,
                    stream,
                };

            var httpRequestMessage = MakeRequest("/api/conversation/talk", HttpMethod.Post, jsonParameters);
            var readAsStreamAsync =
                SendRequest(httpRequestMessage).EnsureSuccessStatusCode().Content.ReadAsStreamAsync();
            readAsStreamAsync.Wait();
            return new PandoraChatResponseHandler(readAsStreamAsync.Result, stream);
        });
    }

    public override List<ModelDescription> GetModels()
    {
        var httpRequestMessage = MakeRequest("/api/models", HttpMethod.Get);
        return (ReadJObjectFromResponse(SendRequest(httpRequestMessage).EnsureSuccessStatusCode())["model"] ?? throw new InvalidOperationException())
            .Value<JArray>()!.Select(jToken =>
                ModelDescription.FromJson(jToken as JObject ?? throw new InvalidOperationException())).ToList();
    }

    public override Conversation GetConversation(Guid conversationId)
    {
        var httpRequestMessage = MakeRequest("/api/conversation/" + conversationId, HttpMethod.Get);
        return Conversation.FromJson(ReadJObjectFromResponse(SendRequest(httpRequestMessage).EnsureSuccessStatusCode()));
    }

    public override void DeleteConversation(Guid conversationId)
    {
        SendRequest(MakeRequest("/api/conversation/" + conversationId, HttpMethod.Delete)).EnsureSuccessStatusCode();
    }

    public override void SetConversationTitle(string title, Guid conversationId)
    {
        throw new NotImplementedException();
    }

    public PandoraGPTModelProvider(string pandoraLaunchCmd,int pandoraServicePort, WebProxy? proxy, string username, string password) : base(proxy, username, password)
    {
        _pandoraLaunchCmd = pandoraLaunchCmd;
        _pandoraServicePort = pandoraServicePort;
        DoLogin(username, password);
    }
}