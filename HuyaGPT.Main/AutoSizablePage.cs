using System.Windows;
using System.Windows.Controls;

namespace HuyaGPT;

public abstract class AutoSizablePage : Page
{
    private readonly ColumnDefinition _widthDefinition;
    private readonly Control _heightControl;

    protected AutoSizablePage(ColumnDefinition widthDefinition, Control heightControl)
    {
        _widthDefinition = widthDefinition;
        _heightControl = heightControl;
    }

    public void UpdateSize()
    {
        var frameworkElement = GetUpdateControl();
        frameworkElement.Width = (int)(_widthDefinition.ActualWidth);
        // StackPanel.Height = _baseControl.ActualHeight;
        Width = frameworkElement.Width;
        Height = _heightControl.ActualHeight;
    }
    
    protected abstract FrameworkElement GetUpdateControl(); 
}