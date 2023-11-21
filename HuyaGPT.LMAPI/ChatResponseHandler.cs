namespace HuyaGPT.LMAPI;

public abstract class ChatResponseHandler
{
    public event EventHandler<OnChatProgressUpdateArgs>? OnChatProgressUpdate;
    public event EventHandler? OnChatCompleted;
    public event EventHandler? OnChatError;
    protected readonly Stream Stream;
    protected readonly bool IsSteam;

    protected ChatResponseHandler(Stream stream, bool isSteam)
    {
        Stream = stream;
        IsSteam = isSteam;
    }
    public abstract void StartReadResponseAsync();

    protected void ChatProgressUpdate(OnChatProgressUpdateArgs args)
    {
        OnChatProgressUpdate?.Invoke(this, args);
    }

    protected void ChatCompleted()
    {
        OnChatCompleted?.Invoke(this, EventArgs.Empty);
    }

    protected void ChatError()
    {
        OnChatError?.Invoke(this, EventArgs.Empty);
    }
}
public record OnChatProgressUpdateArgs(string NewChatMessage, Guid MessageId, Guid? ConversationId, bool IsComplete);
public delegate void OnChatProgressUpdate(object? sender, OnChatProgressUpdateArgs args);