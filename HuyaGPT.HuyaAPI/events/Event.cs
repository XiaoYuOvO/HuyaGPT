namespace HuyaGPT.HuyaAPI.events;

public abstract class Event {
    public string EventName { get; }

    protected Event(string eventName) {
        EventName = eventName;
    }
}