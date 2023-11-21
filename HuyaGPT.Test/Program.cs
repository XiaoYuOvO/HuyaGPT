// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Text;
using HuyaGPT.HuyaAPI.events;
using HuyaGPT.HuyaAPI.websocket;
using HuyaGPT.LMAPI;
using HuyaGPT.LMAPI.pandora;
using log4net.Config;

// var pandoraGptModelProvider = new PandoraGPTModelProvider("pandora.exe",6666,null,"","");
// var chatManager = new ChatManager<PandoraChatResponseHandler>(pandoraGptModelProvider)
// {
//     Stream = true
// };
// chatManager.AddProgressUpdateListener((sender, updateArgs) => { Console.WriteLine(updateArgs.NewChatMessage); });
// chatManager.AddCompleteListener((sender, updateArgs) => { Console.WriteLine("Chat completed");});
// chatManager.Talk("""
// 这是一个Json对象: 
// {
//       "role": "assistant",
//       "name": null,
//       "metadata": {}
//     }
// """);
// chatManager.Talk("请问刚才那个Json对象的 role 属性是什么");
