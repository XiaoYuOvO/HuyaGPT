using JWT.Serializers;

namespace HuyaGPT.HuyaAPI.util;

using System;
using System.Collections.Generic;
using System.Text;
using JWT;
using JWT.Algorithms;
using JWT.Builder;

public class ParamsUtil
{
    public static Dictionary<string, object> GetWebSocketJwtParamsMap(string appId, string secret, long roomId)
    {
        var currentTimeMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var expireTimeMillis = currentTimeMillis + 10 * 60 * 1000; // 超时时间: 通常设置10分钟有效，即exp=iat+600，注意不少于当前时间且不超过当前时间60分钟
        var iat = DateTimeOffset.FromUnixTimeMilliseconds(currentTimeMillis).LocalDateTime;
        var exp = DateTimeOffset.FromUnixTimeMilliseconds(expireTimeMillis).LocalDateTime;
        try
        {
            // 生成JWT凭证
            HMACSHA256Algorithm algorithm = new HMACSHA256Algorithm();
            IJwtEncoder encoder = new JwtEncoder(algorithm, new SystemTextSerializer(), new JwtBase64UrlEncoder());
            var sToken = encoder.Encode(new Dictionary<string, object>
            {
                { "appId", appId },
                { "iat", currentTimeMillis / 1000 },
                { "exp", expireTimeMillis / 1000}
            }, Encoding.UTF8.GetBytes(secret));
            var authMap = new Dictionary<string, object>
            {
                { "exp", expireTimeMillis / 1000 },
                { "iat", currentTimeMillis / 1000 },
                { "sToken", sToken },
                { "appId", appId },
                { "do", "comm" },
                { "roomId", roomId }
            };
            return authMap;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return new Dictionary<string, object>();
    }

    public static string MapToUrlString(Dictionary<string, object> paramsMap, bool hasQuerySymbol)
    {
        var preStr = new StringBuilder(hasQuerySymbol ? "?" : "");
        foreach (var entry in paramsMap)
        {
            try
            {
                preStr.Append(entry.Key)
                       .Append('=')
                       .Append(Uri.EscapeDataString(entry.Value.ToString() ?? string.Empty))
                       .Append('&');
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return preStr.ToString().TrimEnd('&');
    }
}