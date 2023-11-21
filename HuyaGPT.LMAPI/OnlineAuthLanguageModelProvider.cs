using System.Net;
using Newtonsoft.Json;

namespace HuyaGPT.LMAPI;

public abstract class OnlineAuthLanguageModelProvider<T> : NetworkBasedModelProvider<T> where T : ChatResponseHandler
{
    private readonly string _username;
    private readonly string _password;

    protected OnlineAuthLanguageModelProvider(WebProxy? proxy,string username, string password) : base(proxy)
    {
        _username = username;
        _password = password;
    }

    protected abstract void DoLogin(string username, string password);
}