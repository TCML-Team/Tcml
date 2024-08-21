using Avalonia.Controls;
using Avalonia.Input;
using FluentAvalonia.UI.Controls;
using Tkmm.Attributes;
using Tkmm.Helpers;
using Tkmm.ViewModels.Pages;

namespace Tkmm.Views.Pages;

public partial class GameBananaPageView : UserControl
{
    public GameBananaPageView()
    {
        InitializeComponent();
        DataContext = new GameBananaPageViewModel();
    }

    private async void TextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is GameBananaPageViewModel vm) {
            await vm.Search(ModsViewer);
        }
    }
}
