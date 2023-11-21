namespace HuyaGPT.Util;

public static class LazyUtils
{
    public static T Make<T>(T src, Action<T> makeFunc)
    {
        makeFunc(src);
        return src;
    }
}