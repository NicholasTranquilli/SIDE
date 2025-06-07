using PluginLib;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SIDE.ViewModels
{
    public class TextEditorViewModel : ViewModelBase
    {
        // Plugin Data
        public HostToPluginData rPluginData;
        // End Plugin Data

        public string SyntaxDefinitionFilePath { get; set; } = string.Empty;

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set => this.RaiseAndSetIfChanged(ref _filePath, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private string _content;
        public string Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    this.RaiseAndSetIfChanged(ref _content, value);
                    rPluginData.EditorText = value;
                }
            }
        }

        public TextEditorViewModel(HostToPluginData data)
        {
            this.rPluginData = data;

            this.Name = string.Empty;
            this.Content = string.Empty;
            this.FilePath = string.Empty;
        }

        public TextEditorViewModel(HostToPluginData data, string name, string content, string filePath)
        {
            this.rPluginData = data;

            this.Name = name;
            this.Content = content;
            this.FilePath = filePath;
        }
    }
}
