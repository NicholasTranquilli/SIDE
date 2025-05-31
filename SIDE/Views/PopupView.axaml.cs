using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace SIDE.Views;

public partial class PopupView : Window
{
    public PopupView()
    {
        InitializeComponent();
    }

    public PopupView(string message) : this()
    {
        MessageTextBlock.Text = message;
    }

    private void ButtonOk_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}