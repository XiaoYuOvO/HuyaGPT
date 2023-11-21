namespace HuyaGPT.HuyaAPI.events;
public delegate void EventListener<in T>(T theEvent) where T : Event;