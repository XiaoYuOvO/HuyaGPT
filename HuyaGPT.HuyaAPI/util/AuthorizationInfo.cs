namespace HuyaGPT.HuyaAPI.util;

public class AuthorizationInfo
{
    public AuthorizationInfo(string appId, string secret, long roomId)
    {
        AppId = appId;
        Secret = secret;
        RoomId = roomId;
    }

    public string AppId { get; }

    public long RoomId { get; }

    public string Secret { get; }
}