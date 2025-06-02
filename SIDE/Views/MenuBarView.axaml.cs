using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SIDE.ViewModels;
using Avalonia.Metadata;
using Avalonia.Platform.Storage;
using System;
using System.IO;
using System.Reflection;
using PluginLib;
namespace SIDE.Views;

public partial class MenuBarView : UserControl
{
    public static void LoadAndShowPlugin(string pluginPath, Window parentWindow)
    {
        try
        {
            var asm = Assembly.LoadFrom(pluginPath);

            foreach (var type in asm.GetTypes())
            {
                if (typeof(IPluginWindow).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    if (Activator.CreateInstance(type) is IPluginWindow plugin)
                    {
                        var window = plugin.CreateWindow();
                        window.Show(parentWindow);
                        return;
                    }
                }
            }

            Console.WriteLine("No valid plugin found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Plugin load failed: {ex.Message}");
        }
    }

    public MenuBarView()
    {
        InitializeComponent();
    }

    private void MenuItemNew_Clicked(object? sender, RoutedEventArgs e)
    {
        var window = this.VisualRoot as Window;
        if (DataContext is MainViewModel vm && window is not null)
        {
            string newTabName = $"Untitled {vm.Items.Count + 1}";
            string newTabContent = string.Empty;

            vm.AppendTab(newTabName, newTabContent, string.Empty);
        }
    }

    private void MenuItemOpen_Clicked(object? sender, RoutedEventArgs e)
    {
        var window = this.VisualRoot as Window;
        if (DataContext is MainViewModel vm && window is not null)
        {
            vm.OnOpenFile.Execute(window).Subscribe();
        }
    }

    private void MenuItemSave_Clicked(object? sender, RoutedEventArgs e)
    {
        var window = this.VisualRoot as Window;
        if (DataContext is MainViewModel vm && window is not null)
        {
            vm.QuickSave(window);
        }
    }

    private void MenuItemSaveAs_Clicked(object? sender, RoutedEventArgs e)
    {
        var window = this.VisualRoot as Window;
        if (DataContext is MainViewModel vm && window is not null)
        {
            vm.OnSaveFileAs.Execute(window).Subscribe();
        }
    }

    private void MenuItemSetBuildScript_Clicked(object? sender, RoutedEventArgs e)
    {
        var window = this.VisualRoot as Window;
        if (DataContext is MainViewModel vm && window is not null)
        {
            vm.OnSetBuildScript.Execute(window).Subscribe();
        }
    }

    private void MenuItemBuild_Clicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.OnBuild.Execute().Subscribe();
    }

    private void LoadPlugin_Click(object? sender, RoutedEventArgs e)
    {
        var window = this.VisualRoot as Window;
        if (DataContext is MainViewModel vm && window is not null)
        {
            // TODO: Change make dynamic path
            var path = @"D:\Resources\SIDE Plugins\ParseAssist.dll";
            MenuBarView.LoadAndShowPlugin(path, window);
        }
    }
}