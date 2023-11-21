namespace HuyaGPT.HuyaAPI;

public abstract class HuyaUser {
    public string NickName { get; }
    public string UniqueId { get; }

    protected HuyaUser(string nickName, string uniqueID) {
        NickName = nickName;
        UniqueId = uniqueID;
    }
    private bool Equals(HuyaUser other)
    {
        return NickName == other.NickName && UniqueId == other.UniqueId;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((HuyaUser)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(NickName, UniqueId);
    }
}
