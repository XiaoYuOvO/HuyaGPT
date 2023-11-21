namespace HuyaGPT.HuyaAPI;

public class OnlineUser : HuyaUser
{
    public OnlineUser(string nickName, string uniqueId, int fansLevel, int nobleLevel, int senderLevel, string badgeName, string avatarUrl)
        : base(nickName, uniqueId)
    {
        FansLevel = fansLevel;
        NobleLevel = NobleType.GetFromLevel(nobleLevel);
        SenderLevel = senderLevel;
        BadgeName = badgeName;
        AvatarUrl = avatarUrl;
    }

    public int FansLevel { get; }

    public NobleType NobleLevel { get; }

    public int SenderLevel { get; }

    public string BadgeName { get; }
    public string AvatarUrl { get; }

    public class Builder
    {
        private int _fansLevel = -1;
        private int _nobleLevel = -1;
        private int _senderLevel = -1;
        private string _badgeName = string.Empty;
        private readonly string _nickName;
        private readonly string _unionId;
        private string _avatarUrl = string.Empty;

        private Builder(string nickName, string unionId)
        {
            _nickName = nickName;
            _unionId = unionId;
        }

        public static Builder Of(string nickName, string unionId)
        {
            return new Builder(nickName, unionId);
        }

        public Builder SetBadgeName(string badgeName)
        {
            _badgeName = badgeName;
            return this;
        }

        public Builder SetFansLevel(int fansLevel)
        {
            _fansLevel = fansLevel;
            return this;
        }

        public Builder SetNobleLevel(int nobleLevel)
        {
            _nobleLevel = nobleLevel;
            return this;
        }

        public Builder SetSenderLevel(int senderLevel)
        {
            _senderLevel = senderLevel;
            return this;
        }

        public Builder AvatarUrl(string avatarUrl)
        {
            _avatarUrl = avatarUrl;
            return this;
        }

        public OnlineUser Build()
        {
            return new OnlineUser(_nickName, _unionId, _fansLevel, _nobleLevel, _senderLevel, _badgeName, _avatarUrl);
        }
    }

    public override string ToString()
    {
        return $"OnlineUser{{ userName={NickName}, fansLevel={FansLevel}, nobleLevel={NobleLevel}, senderLevel={SenderLevel} }}";
    }
}