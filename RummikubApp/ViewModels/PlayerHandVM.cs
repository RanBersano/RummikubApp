using CommunityToolkit.Mvvm.ComponentModel;
using RummikubApp.ModelLogics;
using System.Collections.ObjectModel;
using System.Windows.Input;
namespace RummikubApp.ViewModels;
public partial class PlayerHandVM : ObservableObject
{
    #region Fields
    private readonly Game game;
    #endregion
    #region Commands
    private readonly Command<int> tileTappedCommand;
    public ICommand TileTappedCommand => tileTappedCommand;
    #endregion
    #region Properties
    public ObservableCollection<Tile> Board => game.UiBoard;
    #endregion
    #region Constructor
    public PlayerHandVM(Game game)
    {
        this.game = game;
        game.UiChanged += OnUiChanged;
        tileTappedCommand = new Command<int>(i => this.game.TileTapped(i));
        this.game.RefreshUi();
    }
    #endregion
    #region Private Methods
    private void OnUiChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(Board));
    }
    #endregion
}