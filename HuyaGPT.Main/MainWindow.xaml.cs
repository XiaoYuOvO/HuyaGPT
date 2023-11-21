using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HandyControl.Collections;
using HandyControl.Controls;
using HuyaGPT.HuyaAPI.events;
using HuyaGPT.HuyaAPI.websocket;
using HuyaGPT.LMAPI;
using HuyaGPT.LMAPI.pandora;
using HuyaGPT.Util;
using log4net.Config;
using MahApps.Metro.Controls;

namespace HuyaGPT;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private const char TriggerWord = '@';
    private readonly ManualObservableCollection<MessageInfo> _messageQueue = new();
    private readonly ChatManager<PandoraChatResponseHandler> _chatManager;
    private readonly Dictionary<string, BitmapImage> _avatarCache = new();
    private readonly AtomicBool _isTalking = new();
    public MainWindow()
    {
        InitializeComponent();
        MessageQueue.ItemsSource = _messageQueue;

        var huyaAppId = HuyaGPTConfig.Instance.HuyaAppId;
        var huyaAppSecret = HuyaGPTConfig.Instance.HuyaAppSecret;
        var huyaRoom = HuyaGPTConfig.Instance.HuyaRoomId;
        if (huyaAppId.IsEmpty() || huyaAppSecret.IsEmpty())
        {
            HandyControl.Controls.MessageBox.Error("未输入虎牙AppId和AppSecret,软件未启动,请前往设置填写并保存后重启程序");
            return;
        }

        if (HuyaGPTConfig.Instance.ChatGptEmail.IsEmpty() || HuyaGPTConfig.Instance.ChatGptPassword.IsEmpty())
        {
            HandyControl.Controls.MessageBox.Error("未输入ChatGPT账号和密码,软件未启动,请前往设置填写并保存后重启程序");
            return;
        }
        _chatManager = new(new PandoraGPTModelProvider("pandora.exe", HuyaGPTConfig.Instance.PandoraServicePort,
            new WebProxy(HuyaGPTConfig.Instance.PandoraProxy.IsEmpty() ? null : new Uri(HuyaGPTConfig.Instance.PandoraProxy)), HuyaGPTConfig.Instance.ChatGptEmail, HuyaGPTConfig.Instance.ChatGptPassword))
        {
            Stream = true
        };
        _chatManager.AddCompleteListener((sender, args) =>
        {
            _isTalking.Value = false;
            this.Invoke(UpdateNewChat);
            UpdateQueueSize();
            LogWindow.Instance.Info("Chat task completed");
        });
        var eventClient = new EventClient(new Uri("ws://ws-apiext.huya.com/index.html"),huyaAppId, huyaAppSecret, huyaRoom)
            {
                Logger = LogWindow.Instance
            };
        eventClient.RegisterListener(EventTypes.MessageEvent, OnNextMessage);
        eventClient.AddOpenListener(() => LogWindow.Instance.Info("WS Connection opened"));
        eventClient.AddErrorListener(exception => LogWindow.Instance.Info(exception.ToString()));
        eventClient.ConnectAsync();
        LogWindow.Instance.Info("Injecting initial message");
        var message = HuyaGPTConfig.Instance.InitialPrompt.Replace("$CurrentTime",DateTime.Now.ToString(CultureInfo.CurrentUICulture));
        LogWindow.Instance.Info(message);
        _chatManager.Talk(message);
        _isTalking.Value = true;
    }

    private void OnNextMessage(MessageEvent messageEvent)
    {
        if (_messageQueue.Count > HuyaGPTConfig.Instance.MaxQueueSize - 1 || !messageEvent.Content.StartsWith(TriggerWord))
        {
            return;
        }
        var messageInfo = new MessageInfo(messageEvent.User.NickName, messageEvent.Content.Trim(TriggerWord), messageEvent.User.AvatarUrl, messageEvent.User.UniqueId);
        if (_isTalking.Value)
        {
            _messageQueue.Add(messageInfo);
            UpdateQueueSize();
            this.Invoke(UpdateNewChat);
        }
        else
        {
            StartTalk(messageInfo);
        }
    }

    private void UpdateNewChat()
    {
        if (_isTalking.Value || _messageQueue.Count == 0) return;
        var messageInfo = _messageQueue[0];
        _messageQueue.RemoveAt(0);
        StartTalk(messageInfo);
    }

    private async void StartTalk(MessageInfo messageInfo)
    {
        LogWindow.Instance.Info("Start to talk: " + messageInfo.Message);
        ChatPanel.Children.Add(new ChatContent(messageInfo));
        var chatGptMessage = ChatContent.CreateChatGptMessage("");
        ChatPanel.Children.Add(chatGptMessage);
        
        _chatManager.ClearProgressUpdateListener();
        var count = 0;
        _chatManager.AddProgressUpdateListener((sender, args) =>
        {
            count++;
            if (count <= 5 && !args.IsComplete) return;
            chatGptMessage.ChatText.Invoke(() =>
            {
                chatGptMessage.ChatText.Text = args.NewChatMessage;
                ChatPanelScroller.ScrollToBottom();
            });
            count = 0;
        });
        try
        {
            ChatPanelScroller.ScrollToBottom();
            _chatManager.Talk($"[{messageInfo.UserName}]: {messageInfo.Message}");
        }
        catch (Exception e)
        {
            LogWindow.Instance.Error(e.ToString());
            chatGptMessage.ChatText.Text = e.ToString();
            throw;
        }
        _isTalking.Value = true;
    }

    private void MessageQueue_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        (MessageQueue.View as GridView).Columns[1].Width = e.NewSize.Width - 80;
    }

    private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
    {
        var list = MessageQueue.SelectedItems.Cast<MessageInfo>().ToList();
        foreach (var messageInfo in list)
        {
            _messageQueue.Remove(messageInfo);
            UpdateQueueSize();
        }
    }

    private void UpdateQueueSize()
    {
        this.Invoke(() => { this.MessageQueueTip.Content = $"排队任务:{_messageQueue.Count}/{HuyaGPTConfig.Instance.MaxQueueSize}"; });
    }
}