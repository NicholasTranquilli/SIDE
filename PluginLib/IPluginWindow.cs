
using System.ComponentModel;

namespace PluginLib
{
    // All host data that made available to the plugin
    public class HostToPluginData : INotifyPropertyChanged
    {
        public string _editorText;
        public string EditorText
        {
            get => _editorText;
            set
            {
                _editorText = value;
                OnPropertyChanged(nameof(EditorText));
            }
        }

        public HostToPluginData() { }

        public HostToPluginData(string _editorText) 
        {
            this.EditorText = _editorText;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public interface IPluginWindow
    {
        public string PluginName { get; }

        public HostToPluginData PluginData { get; set; }
        //{
        //    get => PluginData;
        //    set => this.OnDataUpdate(value);
        //}

        public Avalonia.Controls.Window CreateWindow(HostToPluginData data);

        // Data update callback
        //public void OnDataUpdate(HostToPluginData data);
        public void OnDataUpdate(ref HostToPluginData _data, HostToPluginData data);
    }
}
