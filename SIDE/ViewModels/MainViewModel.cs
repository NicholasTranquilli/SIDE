using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using DynamicData;
using ReactiveUI;

namespace SIDE.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<TextEditorViewModel> _items;
        public ObservableCollection<TextEditorViewModel> Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedTabIndex, value);
        }

        public void AppendTab(string tabName, string tabContent, string filePath)
        {
            Items.Add(new TextEditorViewModel(tabName, tabContent, filePath));
            
            SelectedTabIndex = Items.Count - 1;
        }

        public ReactiveCommand<Window, Unit> OnOpenFile { get; }
        private async Task OpenFile(Window window)
        {
            var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open File",
                AllowMultiple = false
            });

            if (files.Count >= 1)
            {
                await using var stream = await files[0].OpenReadAsync();
                using var reader = new StreamReader(stream);

                this.AppendTab(files[0].Name, await reader.ReadToEndAsync(), files[0].Path.AbsolutePath);
            }
        }

        public async void QuickOpen(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                try
                {
                    await using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using var reader = new StreamReader(stream);

                    string content = await reader.ReadToEndAsync();
                    this.AppendTab(Path.GetFileName(filePath), content, filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading file: {ex.Message}");
                }
            }
        }

        public ReactiveCommand<Window, Unit> OnSaveFileAs { get; }
        private async Task SaveFileAs(Window window)
        {
            string currentTabContent = this.Items[this.SelectedTabIndex].Content;

            var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save File As...",
                SuggestedFileName = this.Items[this.SelectedTabIndex].Name,
                ShowOverwritePrompt = true,
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("Text Files")
                    {
                        Patterns = new[] { "*.txt" }
                    }
                }
            });

            if (file is not null)
            {
                this.Items[this.SelectedTabIndex].FilePath = file.Path.AbsolutePath;
                this.Items[this.SelectedTabIndex].Name = file.Name;

                await using var stream = await file.OpenWriteAsync();
                using var writer = new StreamWriter(stream);

                await writer.WriteAsync(currentTabContent ?? string.Empty);
            }
        }
        
        public async void QuickSave(Window window)
        {
            var currentItem = this.Items[this.SelectedTabIndex];
            var filePath = Uri.UnescapeDataString(currentItem.FilePath);
            var content = currentItem.Content;

            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    await using var stream = File.Open(filePath, FileMode.Create, FileAccess.Write);
                    using var writer = new StreamWriter(stream);
                    await writer.WriteAsync(content ?? string.Empty);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving file: {ex.Message}");
                }
            }
            else
            {
                OnSaveFileAs.Execute(window).Subscribe();
            }
        }

        public MainViewModel()
        {
            Items = new ObservableCollection<TextEditorViewModel>();

            AppendTab(
                "Welcome",
                "Roadmap:\n" +
                "- Keyboard Shortcuts\n" +
                "- Robust Plugin System\n" +
                "- Theme Builder and Importer\n" +
                "- Language features (auto indent, etc...)\n" +
                "- Several Custom Plugins\n" +
                "- File System view\n" +
                "- Custom Build Scripts\n" +
                "- Terminal View\n" +
                "- X button on tabs\n" +
                "- Tab changes notifier\n" +
                "- Better Error Popup Window",
                string.Empty
            );

            SelectedTabIndex = 0; 

            OnOpenFile = ReactiveCommand.CreateFromTask<Window>(OpenFile);
            OnSaveFileAs = ReactiveCommand.CreateFromTask<Window>(SaveFileAs);
        }
    }
}
