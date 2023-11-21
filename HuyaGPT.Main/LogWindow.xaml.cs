using System.Windows;
using HuyaGPT.HuyaAPI.websocket;
using MahApps.Metro.Controls;

namespace HuyaGPT;

public partial class LogWindow : ILogger
{
    public static readonly LogWindow Instance = new();
    private LogWindow()
    {
        InitializeComponent();
        Show();
    }

    public void Info(string message)
    {
        LogBlock.Invoke(() =>
        {
            LogBlock.Text += message + ("\n");
        });

    }
    
    public void Error(string message)
    {
        LogBlock.Invoke(() => { LogBlock.Text += ("! ") + message + ("\n"); });
    }

    public void Error(string message, Exception exception)
    {
        LogBlock.Invoke(() => { LogBlock.Text += "! " + message + "\n" + exception; });
    }
}