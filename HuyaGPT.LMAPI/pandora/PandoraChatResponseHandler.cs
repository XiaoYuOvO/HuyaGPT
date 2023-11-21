using System.Text;
using HuyaGPT.Util;
using Newtonsoft.Json.Linq;

namespace HuyaGPT.LMAPI.pandora;

public class PandoraChatResponseHandler : ChatResponseHandler
{
    public PandoraChatResponseHandler(Stream stream, bool isSteam) : base(stream, isSteam) {}

    public override void StartReadResponseAsync()
    { 
        Task.Run(() =>
        {
            using var reader = new StreamReader(Stream, Encoding.UTF8);
            while (!reader.EndOfStream)
            {
                JObject? jObject = null;
                var character = reader.ReadLine();
                if (character == null || character.Length == 0) continue;
                if (character.StartsWith("data: "))
                {
                    var data = character.Replace("data:", "").Trim();
                    if (data.Equals("[DONE]"))
                    {
                        ChatCompleted();
                        return;
                    }
                    jObject = JObject.Parse(data);
                }
                else if (character.StartsWith("{"))
                {
                    jObject = JObject.Parse(character);
                }

                if (jObject.ContainsKey("message"))
                {
                    var message = jObject?.GetObject("message");
                    if (message != null && message.GetObject("author").GetString("role").Equals("assistant"))
                    {
                        var responseContent = message.GetObject("content").GetArray("parts")[0].Value<string>();
                        var isComplete = message.GetString("status").Equals("finished_successfully");
                        if (responseContent != null)
                        {
                            ChatProgressUpdate(new OnChatProgressUpdateArgs(responseContent, message.GetGuid("id"),
                                jObject != null && jObject.ContainsKey("conversation_id")
                                    ? jObject.GetGuid("conversation_id")
                                    : null,
                                isComplete));
                        }

                        if (!IsSteam)
                        {
                            ChatCompleted();
                            return;
                        }
                    }    
                }else if (jObject.ContainsKey("moderation_response"))
                {
                    ChatError();
                    return;
                }
                
            }    
        });
    }
}

