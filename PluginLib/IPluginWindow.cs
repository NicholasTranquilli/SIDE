
namespace PluginLib
{
    public interface IPluginWindow
    {
        string Name { get; }
        Avalonia.Controls.Window CreateWindow();
    }
}
