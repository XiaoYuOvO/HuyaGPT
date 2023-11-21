using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;

namespace HuyaGPT;

public partial class ConfigPage
{
    public readonly ConfigModel Model = new();

    public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
        nameof(Model), typeof(ConfigModel), typeof(ConfigPage), new PropertyMetadata(default(ConfigModel)));

    public ConfigPage(ColumnDefinition widthDefinition, Control heightControl) : base(widthDefinition,heightControl)
    {
        InitializeComponent();
        PropertyGrid.SelectedObject = Model;
        PropertyGrid.ShowSortButton = false;
    }
    
    protected override FrameworkElement GetUpdateControl()
    {
        return Panel;
    }
}

public class ConfigModel
{
    internal void MarkDirty()
    {
        try
        {
            HuyaGPTConfig.Instance.SaveConfiguration();
        }
        catch (Exception e)
        {
            Growl.Error(e.Message);
        }
    }
    
    public string PandoraProxy { get; set; }
    public string ChatGptEmail { get; set; }
    public string ChatGptPassword { get; set; }
    public int MaxQueueSize { get; set; }

    public string HuyaAppId
    {
        get => HuyaGPTConfig.Instance.HuyaAppId;
        set
        {
            HuyaGPTConfig.Instance.HuyaAppId = value;
            MarkDirty();
        }
    }
    
    public string HuyaAppSecret
    {
        get => HuyaGPTConfig.Instance.HuyaAppSecret;
        set
        {
            HuyaGPTConfig.Instance.HuyaAppSecret = value;
            MarkDirty();
        }
    }
    
    public string InitialPrompt
    {
        get => HuyaGPTConfig.Instance.InitialPrompt;
        set
        {
            HuyaGPTConfig.Instance.InitialPrompt = value;
            MarkDirty();
        }
    }
    
    public int HuyaRoomId
    {
        get => HuyaGPTConfig.Instance.HuyaRoomId;
        set
        {
            HuyaGPTConfig.Instance.HuyaRoomId = value;
            MarkDirty();
        }
    }
    
    public int PandoraServicePort
    {
        get => HuyaGPTConfig.Instance.PandoraServicePort;
        set
        {
            HuyaGPTConfig.Instance.PandoraServicePort = value;
            MarkDirty();
        }
    }
}