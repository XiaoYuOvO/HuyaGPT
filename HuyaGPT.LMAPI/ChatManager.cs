using System.Diagnostics;

namespace HuyaGPT.LMAPI;

public class ChatManager<T> where T : ChatResponseHandler
{
    private readonly ILanguageModelProvider<T> _languageModelProvider;
    public bool Stream;
    private Guid? _conversationId;
    private Guid _lastMessageId = Guid.NewGuid();
    private readonly List<OnChatProgressUpdate> _chatProgressListeners = new();
    private readonly List<EventHandler> _chatCompleteListeners = new();

    public ChatManager(ILanguageModelProvider<T> languageModelProvider)
    {
        _languageModelProvider = languageModelProvider;
    }

    public void Talk(string message)
    {
        Task.Run(async () =>
        {
            var chatProgressHandler =
                await _languageModelProvider.Talk(message, Guid.NewGuid(), _lastMessageId, _conversationId, Stream);
            chatProgressHandler.OnChatProgressUpdate += OnChatProgress;
            chatProgressHandler.OnChatCompleted += OnChatComplete;
            chatProgressHandler.StartReadResponseAsync();
        });
    }

    public void AddProgressUpdateListener(OnChatProgressUpdate listener)
    {
        _chatProgressListeners.Add(listener);
    }
    
    public void AddCompleteListener(EventHandler listener)
    {
        _chatCompleteListeners.Add(listener);
    }

    public void ClearProgressUpdateListener()
    {
        _chatProgressListeners.Clear();
    }

    private void OnChatProgress(object? sender, OnChatProgressUpdateArgs args)
    {
        if (args.ConversationId != null)
        {
            _conversationId = (Guid)args.ConversationId;
            Debug.WriteLine("ConversationId: " + _conversationId);
        }

        _lastMessageId = args.MessageId;
        Debug.WriteLine("MessageId: " + args.MessageId);
        foreach (var onChatProgressUpdate in _chatProgressListeners)
        {
            onChatProgressUpdate(sender, args);
        }
    }

    public void RemoveProgressListener(OnChatProgressUpdate listener)
    {
        _chatProgressListeners.Remove(listener);
    }
    
    public void RemoveCompleteListener(EventHandler listener)
    {
        _chatCompleteListeners.Remove(listener);
    }

    private void OnChatComplete(object? sender, EventArgs args)
    {
        foreach (var chatCompleteListener in _chatCompleteListeners)
        {
            chatCompleteListener(sender, args);
        }
    }
}