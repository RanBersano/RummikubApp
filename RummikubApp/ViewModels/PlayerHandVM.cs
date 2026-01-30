using CommunityToolkit.Mvvm.ComponentModel;
using RummikubApp.ModelLogics;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RummikubApp.ViewModels;

public partial class PlayerHandVM : ObservableObject
{
    private readonly Game game;

    public ObservableCollection<Tile> Board => game.UiBoard;

    private readonly Command<int> tileTappedCommand;
    public ICommand TileTappedCommand => tileTappedCommand;

    public PlayerHandVM(Game game)
    {
        this.game = game;

        game.UiChanged += OnUiChanged;

        tileTappedCommand = new Command<int>(i => this.game.TileTapped(i));

        this.game.RefreshUi();
    }

    private void OnUiChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(Board));
    }
}
