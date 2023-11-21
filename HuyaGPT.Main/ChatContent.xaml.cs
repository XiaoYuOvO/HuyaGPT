using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HuyaGPT;

public partial class ChatContent : UserControl
{
    private static readonly SolidColorBrush ChatGptBackgroundBrush = new(Color.FromRgb(19, 0xC3, 0x7D));
    public ChatContent()
    {
        InitializeComponent();
    }
    
    public ChatContent(MessageInfo messageInfo)
    {
        InitializeComponent();
        this.ChatText.Text = messageInfo.Message;
        UserName.Content = messageInfo.UserName;
        this.Gravatar.Id = messageInfo.UserName;
    }
    
    private ChatContent(string message)
    {
        InitializeComponent();
        ChatText.Text = message;
    }

    public static ChatContent CreateChatGptMessage(string message)
    {
        return new ChatContent(message)
        {
            Gravatar =
            {
                Background = ChatGptBackgroundBrush
            },
            UserName =
            {
                Content = "ChatGPT"
            }
        };
    }


    private void ChatText_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        this.Gravatar.Height = Math.Clamp(e.NewSize.Height,60,60);
        Gravatar.Width = Gravatar.Height;
        this.InfoPanel.Height = e.NewSize.Height + 60;
    }
}