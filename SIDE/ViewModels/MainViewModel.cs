using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using DynamicData;
using PluginLib;
using ReactiveUI;
using SIDE.Views;

namespace SIDE.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // Demo Code
        public HostToPluginData rPluginData { get; set; }

        // End Demo Code

        // Tabs //
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
            Items.Add(new TextEditorViewModel(rPluginData, tabName, tabContent, filePath));
            
            SelectedTabIndex = Items.Count - 1;
        }


        // File Commands //
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


        // Terminal Data //
        private string _terminalInput = "";
        public string TerminalInput
        {
            get => _terminalInput;
            set => this.RaiseAndSetIfChanged(ref _terminalInput, value);
        }


        // Build Commands //
        public ReactiveCommand<Unit, Unit> OnBuild { get; }
        private async Task Build()
        {
            if (OperatingSystem.IsWindows())
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/K {TerminalInput}",
                    UseShellExecute = true
                });
            }
            else if (OperatingSystem.IsMacOS())
            {
                Process.Start("open", $"-a Terminal \"bash -c '{TerminalInput}; exec bash'\"");
            }
            else if (OperatingSystem.IsLinux())
            {
                string[] terminals = { "x-terminal-emulator", "gnome-terminal", "konsole", "xfce4-terminal", "xterm" };

                foreach (var term in terminals)
                {
                    try
                    {
                        Process.Start(term, $"-e bash -c \"{TerminalInput}; exec bash\"");
                        break;
                    }
                    catch { }
                }

                // TODO: if linux terminal not detected?
            }
        }

        public ReactiveCommand<Window, Unit> OnSetBuildScript { get; }
        private async Task SetBuildScript(Window window)
        {
            var vm = new SetBuildStringViewModel(TerminalInput);
            var view = new SetBuildStringView
            {
                DataContext = vm
            };

            await view.ShowDialog(window);

            // Update TerminalString after closing
            TerminalInput = vm.TerminalString;
        }

        public MainViewModel()
        {
            Items = new ObservableCollection<TextEditorViewModel>();
            this.rPluginData = new HostToPluginData();

            AppendTab(
                "Welcome",
                "Roadmap:\n" +
                "- Keyboard Shortcuts\n" +
                "- Environment Files And Loading" +
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

            OnBuild = ReactiveCommand.CreateFromTask(Build);
            OnSetBuildScript = ReactiveCommand.CreateFromTask<Window>(SetBuildScript);
            OnOpenFile = ReactiveCommand.CreateFromTask<Window>(OpenFile);
            OnSaveFileAs = ReactiveCommand.CreateFromTask<Window>(SaveFileAs);
        }
    }
}
