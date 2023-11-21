using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace HuyaGPT;

public class HuyaGPTConfig
{
    public static HuyaGPTConfig Instance { get; } = new();
    private const string ConfigPath = "config.json";
    public string HuyaAppId { get; set; }
    public string HuyaAppSecret { get; set; }

    public int HuyaRoomId { get; set; }
    public string InitialPrompt { get; set; }
    public int PandoraServicePort { get; set; }
    public string PandoraProxy { get; set; }
    public string ChatGptEmail { get; set; }
    public string ChatGptPassword { get; set; }
    public int MaxQueueSize { get; set; }

    public HuyaGPTConfig()
    {
        HuyaAppId = "";
        HuyaAppSecret = "";
        HuyaRoomId = 0;
        ChatGptEmail = "";
        ChatGptPassword = "";
        PandoraProxy = "";
        InitialPrompt = File.ReadAllText("prompt.txt");
        PandoraServicePort = 6666;
        MaxQueueSize = 10;
        try
        {
            LoadConfiguration();
        }
        catch (Exception e)
        {
            LogWindow.Instance.Error("Failed to load config", e);
            SaveConfiguration();
        }
    }


    public void SaveConfiguration()
    {
        File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(this), Encoding.UTF8);
    }

    public void LoadConfiguration()
    {
        if (!File.Exists(ConfigPath)) return;
        JsonConvert.PopulateObject(File.ReadAllText(ConfigPath, Encoding.UTF8), this);
    }
}