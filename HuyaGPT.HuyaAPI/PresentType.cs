using HuyaGPT.Util;

namespace HuyaGPT.HuyaAPI;

public class PresentType
{
    private static readonly Dictionary<int, PresentType> IdToGiftType = LazyUtils.Make(
        new Dictionary<int, PresentType>(), (map) =>
        {
            // for (PresentType value : PresentType.values()) {
            //     map.put(value.getGiftId(),value);
            // }
        });

    public string CnName { get; }
    public string EnName { get; }
    public int GiftId { get; }
    public double PrizeYb { get; }
    public int PrizeGoldbean { get; }
    public int PrizeSilverbean { get; }

    PresentType(string cnName, string enName, int giftId, double prizeYb, int prizeGoldbean, int prizeSilverbean)
    {
        CnName = cnName;
        EnName = enName;
        GiftId = giftId;
        PrizeYb = prizeYb;
        PrizeGoldbean = prizeGoldbean;
        PrizeSilverbean = prizeSilverbean;
    }

    public static PresentType GetById(int id)
    {
        return IdToGiftType[id];
    }
}