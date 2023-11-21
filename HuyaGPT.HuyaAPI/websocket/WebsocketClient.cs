using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Timers;

namespace HuyaGPT.HuyaAPI.websocket;

public class WebSocketClient
{
    private readonly ClientWebSocket _webSocket;
    private readonly Uri _serverUri;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly ConcurrentQueue<string> _messageQueue;
    private readonly System.Timers.Timer _pingTimer;

    public WebSocketClient(Uri serverUri)
    {
        _webSocket = new ClientWebSocket();
        //Must set to infinity because the huya's server wont send a pong response
        _webSocket.Options.KeepAliveInterval = Timeout.InfiniteTimeSpan;
        _serverUri = serverUri;
        _cancellationTokenSource = new CancellationTokenSource();
        _messageQueue = new ConcurrentQueue<string>();

        _pingTimer = new System.Timers.Timer(200);
        _pingTimer.Elapsed += PingTimerElapsed;
    }

    public async Task ConnectAsync()
    {
        try
        {
            await _webSocket.ConnectAsync(_serverUri, _cancellationTokenSource.Token);
        }
        catch (WebSocketException e)
        {
            OnException(e);
        }
        OnOpen();

        // 启动接收和发送消息的任务
        var receivingTask = ReceiveMessagesAsync();
        var sendingTask = SendMessagesAsync();

        // 启动定时器
        _pingTimer.Start();

        // 等待任务完成
        await Task.WhenAll(receivingTask, sendingTask);

        // 关闭连接时调用连接关闭事件处理
        OnClose((int)_webSocket.CloseStatus, _webSocket.CloseStatusDescription, false);
    }

    

    public void AddMessageToQueue(string message)
    {
        _messageQueue.Enqueue(message);
    }

    private async Task ReceiveMessagesAsync()
    {
        var buffer = new byte[4096];
        while (_webSocket.State == WebSocketState.Open)
        {
            try
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    // 处理接收到的消息
                    OnMessage(message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    // 关闭连接
                    await _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Closing", _cancellationTokenSource.Token);
                    OnClose(0,result.CloseStatusDescription ?? string.Empty, true);
                    break; // 跳出循环，结束消息接收
                }
            }
            catch (WebSocketException e)
            {
                OnException(e);
            }
        }
    }

 
    private async Task SendMessagesAsync()
    {
        while (_webSocket.State == WebSocketState.Open)
        {
            // 检查消息队列是否有待发送的消息
            if (_messageQueue.TryDequeue(out var message))
            {
                var messageBytes = Encoding.UTF8.GetBytes(message);
                try
                {
                    await _webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, WebSocketMessageFlags.DisableCompression | WebSocketMessageFlags.EndOfMessage,
                        _cancellationTokenSource.Token);
                }
                catch (WebSocketException exception)
                {
                    OnException(exception);
                }

            }

            // 等待一段时间后再次尝试发送消息
            if (_messageQueue.IsEmpty)
            {
                await Task.Delay(10);    
            }
        }
    }
    
    private void PingTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        AddMessageToQueue("ping");
    }

    public async Task Close()
    {
        await _webSocket.CloseAsync(WebSocketCloseStatus.Empty, "bye", _cancellationTokenSource.Token);
        OnClose(0,"bye", false);
    }

    protected virtual void OnMessage(string message)
    {
    }

    protected virtual void OnClose(int code, string message, bool remote)
    {
    }
    protected virtual void OnOpen()
    {
    }

    protected virtual void OnException(Exception exception)
    {
    }
}