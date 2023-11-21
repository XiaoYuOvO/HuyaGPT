using HuyaGPT.Util;

namespace HuyaGPT.HuyaAPI.events;

using Newtonsoft.Json.Linq;

public class MessageEvent : Event
{
    public string Content { get; }
    public OnlineUser User { get; }

    public MessageEvent(JObject json) : base("getMessageNotice")
    {
        var sendNick = json.GetString("sendNick");
        var unionId = json.GetString("unionId");
        var senderLevel = json.GetInt("senderLevel");
        var nobleLevel = json.GetInt("nobleLevel");
        var fansLevel = json.GetInt("fansLevel");

        var userBuilder = OnlineUser.Builder.Of(sendNick, unionId)
            .SetSenderLevel(senderLevel)
            .SetNobleLevel(nobleLevel)
            .SetFansLevel(fansLevel)
            .AvatarUrl(json.GetString("senderAvatarUrl"));

        if (json.GetInt("msgType") == 0)
        {
            var badgeName = json.GetString("badgeName");
            User = userBuilder.SetBadgeName(badgeName).Build();
        }
        else
        {
            User = userBuilder.Build();
        }

        Content = json.GetString("content");
    }

    public override string ToString()
    {
        return $"MessageEvent: {{ [{User.BadgeName}-{User.FansLevel}]({User.NobleLevel.CnName}){User.NickName}:{Content} }}";
    }
}