using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SIDE.ViewModels;
using System.ComponentModel;
using System;
using AvaloniaEdit.Highlighting.Xshd;
using AvaloniaEdit.Highlighting;
using System.Reflection;
using System.Xml;
using System.IO;
using SIDE.Helpers;

namespace SIDE.Views;

public partial class TextEditorView : UserControl
{
    private bool _isUpdatingFromViewModel = false;

    public TextEditorView()
    {
        InitializeComponent();
        this.AttachedToVisualTree += OnAttach;
    }

    private void OnAttach(object? sender, VisualTreeAttachmentEventArgs e)
    {
        TryBind();
    }

    private void LoadSyntaxHighlightingFromFile(string filePath)
    {
        // TODO: Update to custom theme file format
        //if (!File.Exists(filePath))
        //    return;

        //using var reader = XmlReader.Create(filePath);
        //var xshd = HighlightingLoader.LoadXshd(reader);
        //var highlighting = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
        //Editor.SyntaxHighlighting = highlighting;
        
        // UPDATED CODE:
        ThemeImporter myTheme = new ThemeImporter(filePath);
        Editor.SyntaxHighlighting = myTheme.GetThemeData().highlighting;
    }

    private void TryBind()
    {
        if (DataContext is TextEditorViewModel vm)
        {
            Editor.TextChanged -= OnEditorTextChanged;
            vm.PropertyChanged -= OnVmPropertyChanged;

            Editor.Text = vm.Content;

            try
            {
                if (vm.SyntaxDefinitionFilePath != string.Empty)
                    LoadSyntaxHighlightingFromFile(vm.SyntaxDefinitionFilePath);
            }
            catch (Exception ex)
            {
                var errorWindow = new PopupView($"Failed to load syntax file:\n{ex.Message}");
                errorWindow.ShowDialog((Window)this.VisualRoot);
            }

            Editor.TextChanged += OnEditorTextChanged;
            vm.PropertyChanged += OnVmPropertyChanged;
        }
    }

    private void OnEditorTextChanged(object? sender, EventArgs e)
    {
        if (DataContext is TextEditorViewModel vm && !_isUpdatingFromViewModel)
        {
            vm.Content = Editor.Text;
        }
    }

    private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TextEditorViewModel.Content) && DataContext is TextEditorViewModel vm)
        {
            if (Editor.Text != vm.Content)
            {
                _isUpdatingFromViewModel = true;
                Editor.Text = vm.Content;
                _isUpdatingFromViewModel = false;
            }
        }
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        TryBind();
    }
}