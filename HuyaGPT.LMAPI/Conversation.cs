using HuyaGPT.Util;
using Newtonsoft.Json.Linq;

namespace HuyaGPT.LMAPI;

public record Conversation(Guid Id, string Title, DateTime CreateTime, DateTime UpdateTime, Guid CurrentNode)
{
    public static Conversation FromJson(JObject json)
    {
        return new Conversation(Guid.Parse(json.GetString("id")), json.GetString("title"),
            json.GetDateTimeFromStamp("create_time"),//"yyyy-MM-dd'T'HH:mm:ss.ffffffzzz"
            json.GetDateTimeFromStamp("update_time"),//"yyyy-MM-dd'T'HH:mm:sszzz" 
            Guid.Parse(json.GetString("current_node")));
    }
}