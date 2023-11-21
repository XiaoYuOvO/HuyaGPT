using System.Text.Json.Nodes;
using HuyaGPT.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HuyaGPT.LMAPI;

public record ModelDescription(string Slug, string Title, string Description, int MaxTokens)
{
    public static ModelDescription FromJson(JObject jsonObject)
    {
        return new ModelDescription(jsonObject.GetString("slug"), jsonObject.GetString("title"), jsonObject.GetString("description"),
            jsonObject.GetInt("max_tokens"));
    }
}