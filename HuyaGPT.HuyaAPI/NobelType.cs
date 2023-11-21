using HuyaGPT.Util;

namespace HuyaGPT.HuyaAPI;

using System.Collections.Generic;

public class NobleType
{
    private static readonly Dictionary<int, NobleType?> LevelToNobleType = new();
    public static readonly NobleType None = new(0, "无");
    public static readonly NobleType SwordsMan = new(1, "剑士");
    public static readonly NobleType Knight = new(2, "骑士");
    public static readonly NobleType Seignior = new(3, "领主");
    public static readonly NobleType Duke = new(4, "公爵");
    public static readonly NobleType King = new(5, "君王");
    public static readonly NobleType Emperor = new(6, "帝皇");
    public static readonly NobleType LegendaryEmperor = new(7, "超神帝皇");

    public int Level { get; }
    public string CnName { get; }

    private NobleType(int level, string cnName)
    {
        Level = level;
        CnName = cnName;
        LevelToNobleType[Level] = this;
    }

    public static NobleType GetFromLevel(int level)
    {
        return LevelToNobleType[level] ?? None;
    }
}