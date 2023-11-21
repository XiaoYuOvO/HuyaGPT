namespace HuyaGPT.Util;

public class AtomicBool
{
    private long _intValue;

    public bool Value
    {
        get => Interlocked.Read(ref _intValue) == 1;
        set => Interlocked.Exchange(ref _intValue, value ? 1 : 0);
    }
}