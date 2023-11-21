using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;

namespace HuyaGPT.HuyaAPI.events;

public class EventTypes
{
    private static readonly ConcurrentDictionary<string, IEventType> Types = new();
    public static readonly EventType<MessageEvent> MessageEvent = Register("getMessageNotice", json => new MessageEvent(json));

    private static EventType<T> Register<T>(string name, Func<JObject, T> factory) where T : Event
    {
        var eventType = new EventType<T>(factory, name);
        Types.AddOrUpdate(name, eventType, (_, _) => eventType);
        return eventType;
    }

    public static IEventType ByName(string name)
    {
        if (Types.TryGetValue(name, out var eventType))
        {
            return eventType;
        }

        throw new ArgumentException("Invalid event type: " + name);
    }

    public static bool HasRegistered(string eventType)
    {
        return Types.ContainsKey(eventType);
    }
}

public interface IEventType
{
    Event GetFromJson(JObject getObject);
    string Name { get; }
    void CallEventListener(object eventListener, object @event);
}

public class EventType<T> : IEventType where T : Event 
{
    private Func<JObject, T> Factory { get; }
    public string Name { get; }
    public void CallEventListener(object eventListener, object @event)
    {
        ((EventListener<T>)eventListener).Invoke((T)@event);
    }

    internal EventType(Func<JObject, T> factory, string name)
    {
        Factory = factory;
        Name = name;
    }

    public Event GetFromJson(JObject o)
    {
        return Factory(o);
    }

    public override string ToString()
    {
        return Name;
    }
}