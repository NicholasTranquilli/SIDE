using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace SIDE.Views;

public partial class SetBuildStringView : Window
{
    public SetBuildStringView()
    {
        InitializeComponent();
    }

    private void ButtonOk_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}