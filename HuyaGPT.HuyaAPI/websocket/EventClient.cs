using HuyaGPT.HuyaAPI.events;
using HuyaGPT.HuyaAPI.util;
using HuyaGPT.Util;
using Newtonsoft.Json.Linq;

namespace HuyaGPT.HuyaAPI.websocket;

public interface ILogger
{
    void Info(string message);
    void Error(string message);
    void Error(string message, Exception exception);
}

public class EventClient : WebSocketClient
{
    public EventClient(Uri serverUri, string appId, string secret, long roomId) : 
        base(new Uri(serverUri + ParamsUtil.MapToUrlString(ParamsUtil.GetWebSocketJwtParamsMap(appId, secret, roomId),true)))
    { }
    
    // private static readonly ILog Logger = LogManager.GetLogger(typeof(EventHandler));
    private readonly List<CloseListener> _closeListeners = new();
    private readonly List<Action<Exception>> _errorListeners = new();
    private readonly List<Action> _openListeners = new();
    private readonly Dictionary<Type, List<object>> _listeners = new();
    public ILogger? Logger { get; set; }

    public void AddCloseListener(CloseListener listener)
    {
        _closeListeners.Add(listener);
    }

    public void AddErrorListener(Action<Exception> listener)
    {
        _errorListeners.Add(listener);
    }

    public void RegisterListener<T>(EventType<T> eventType, EventListener<T> listener) where T : Event
    {
        var type = eventType.GetType();
        if (!_listeners.ContainsKey(type))
        {
            _listeners[type] = new List<object>();
            SendSubscriptMessage(eventType);
        }
        _listeners[type].Add(listener);
    }

    public void AddOpenListener(Action listener)
    {
        _openListeners.Add(listener);
    }

    protected override void OnOpen()
    {
        foreach (var listener in _openListeners)
        {
            listener();
        }
    }
    
    private void SendSubscriptMessage<T>(EventType<T> eventType) where T : Event 
    {
        var commandObject = new JObject
        {
            ["command"] = "subscribeNotice"
        };
        var subscriptEvents = new JArray { eventType.Name };
        commandObject["data"] = subscriptEvents;
        commandObject["reqId"] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        AddMessageToQueue(commandObject.ToString());
    }

    public delegate void CloseListener(int code, string reason, bool remote);

    protected override void OnClose(int code, string reason, bool remote)
    {
        foreach (var listener in _closeListeners)
        {
            listener(code, reason, remote);
        }
    }

    protected override void OnException(Exception arg0)
    {
        foreach (var listener in _errorListeners)
        {
            listener(arg0);
        }
    }

    protected override void OnMessage(string arg0)
    {
        try
        {
            var res = JObject.Parse(arg0);
            if ("command".Equals(res.GetString("notice")))
            {
                var succeededEvents = new List<IEventType>();
                if (res.GetObject("data").GetString("command").Length == 0)
                {
                    return;
                }
                foreach (var jsonElement in res.GetObject("data").GetArray("data"))
                {
                    if (jsonElement.Type == JTokenType.String)
                    {
                        succeededEvents.Add(EventTypes.ByName(jsonElement.ToString()));
                    }
                }
                var toRemove = _listeners.Keys.Except(succeededEvents.Select(succeededEvent => succeededEvent.GetType())).ToList();
                if (toRemove.Count != 0)
                {
                    Logger?.Error($"Failed to listen for event: {string.Join(", ", toRemove.Select(e => e.Name))}");    
                }
                foreach (var key in toRemove)
                {
                    _listeners.Remove(key);
                }
                Logger?.Info($"Successfully listened: {string.Join(", ", succeededEvents.Select(e => e.Name))}");
            }
            var notice = res.GetString("notice");
            if (EventTypes.HasRegistered(notice))
            {
                var eventType = EventTypes.ByName(notice);
                var data1 = eventType.GetFromJson(res.GetObject("data"));
                if (_listeners.Count != 0)
                {
                    foreach (var eventListener in _listeners[eventType.GetType()])
                    {
                        eventType.CallEventListener(eventListener, data1);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Logger?.Error("Error in processing message", e);
            Console.Error.WriteLine(e.ToString());
            Console.Error.WriteLine(arg0);
        }
    }
}