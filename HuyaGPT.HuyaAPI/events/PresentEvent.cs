using HuyaGPT.Util;

namespace HuyaGPT.HuyaAPI.events;

using Newtonsoft.Json.Linq;

public class PresentEvent : Event
{
    public PresentType PresentType { get; }
    public OnlineUser User { get; }
    public int TotalPay { get; }
    public long Count { get; }
    public long RoomId { get; }

    public PresentEvent(JObject data) : base("getSendItemNotice")
    {
        var badgeName = data.GetString("badgeName");
        var fansLevel = data.GetInt("fansLevel");
        var giftId = data.GetInt("itemId");
        var nobleLevel = data.GetInt("nobleLevel");
        RoomId = data.GetInt("roomId");
        long sendItemCount = data.GetInt("sendItemCount");
        var sendNick = data.GetString("sendNick");
        var sendUnion = data.GetString("unionId");
        var senderLevel = data.GetInt("senderLevel");
        TotalPay = data.GetInt("totalPay");
        Count = sendItemCount;
        User = OnlineUser.Builder.Of(sendNick, sendUnion)
            .SetSenderLevel(senderLevel)
            .SetFansLevel(fansLevel)
            .SetNobleLevel(nobleLevel)
            .SetBadgeName(badgeName)
            .Build();
        PresentType = PresentType.GetById(giftId);
    }

    public override string ToString()
    {
        return $"PresentEvent{{ type={PresentType.CnName}, user={User}, count={Count}, roomId={RoomId}, totalPay={TotalPay} }}";
    }
}