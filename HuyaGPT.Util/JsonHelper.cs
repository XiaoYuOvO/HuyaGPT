using System.Globalization;
using Newtonsoft.Json.Linq;

namespace HuyaGPT.Util;

public static class JsonHelper
{
    public static string GetString(this JObject jsonObject, string name)
    {
        return jsonObject.Value<string>(name) ?? string.Empty;
    }
    public static int GetInt(this JObject jsonObject, string name)
    {
        return jsonObject.Value<int>(name);
    }
    public static DateTime GetDateTime(this JObject jsonObject, string name, string format)
    {
        var value = jsonObject.Value<string>(name) ?? throw new InvalidOperationException();
        return DateTime.ParseExact(value,format, null);
    }
    public static DateTime GetDateTimeFromStamp(this JObject jsonObject, string name)
    {
        var value = jsonObject.Value<double>(name);
        return DateTime.FromOADate(value);
    }
    public static Guid GetGuid(this JObject jsonObject, string name)
    {
        return Guid.Parse(jsonObject.GetString(name));
    }
    public static JObject GetObject(this JObject jsonObject, string name)
    {
        return (jsonObject[name] ?? throw new InvalidOperationException()).Value<JObject>() ?? throw new InvalidOperationException();
    }
    public static JArray GetArray(this JObject jsonObject, string name)
    {
        return (jsonObject[name] ?? throw new InvalidOperationException()).Value<JArray>() ?? throw new InvalidOperationException();
    }
}