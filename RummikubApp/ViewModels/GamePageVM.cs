using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using RummikubApp.ModelLogics;
using RummikubApp.Models;
using RummikubApp.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;
namespace RummikubApp.ViewModels
{
    public partial class GamePageVM : ObservableObject
    {
        private readonly Game game;
        public ObservableCollection<Tile> Board => game.UiBoard;
        public ImageSource? DiscardTileSource => game.DiscardTileSource;
        public string TimeLeft => game.TimeLeft;
        public string MyName => game.MyName;
        public string OtherPlayerName1 => game.GetOtherPlayerName(0);
        public string OtherPlayerName2 => game.GetOtherPlayerName(1);
        public string OtherPlayerName3 => game.GetOtherPlayerName(2);
        public bool IsMyTurn => game.IsFull && game.CurrentPlayerName == game.MyName;
        public bool IsPlayer1Turn => game.CurrentPlayerName == game.GetOtherPlayerName(0);
        public bool IsPlayer2Turn => game.CurrentPlayerName == game.GetOtherPlayerName(1);
        public bool IsPlayer3Turn => game.CurrentPlayerName == game.GetOtherPlayerName(2);
        private readonly Command<int> tileTappedCommand;
        public ICommand TileTappedCommand => tileTappedCommand;
        public ICommand TakeDiscardCommand { get; }
        public ICommand DiscardSelectedCommand { get; }
        public GamePageVM(Game game, Grid deckGrid)
        {
            this.game = game;
            game.OnGameChanged += OnGameChanged;
            game.UiChanged += OnUiChanged;
            game.TimeLeftChanged += OnTimeLeftChanged;
            game.GameOver += GameIsOver;
            game.InitGrid(deckGrid);
            tileTappedCommand = new Command<int>(i => this.game.TileTapped(i));
            TakeDiscardCommand = new Command(
                () => this.game.DoTakeDiscard(),
                () => game.CanTakeDiscard());
            DiscardSelectedCommand = new Command(
                () => this.game.DoDiscardSelected(),
                () => IsMyTurn && game.HasDrawnThisTurn);
            if (!this.game.IsHostUser)
                this.game.UpdateGuestUser(OnJoinComplete);
            this.game.RefreshUi();
        }
        private void GameIsOver(object? sender, bool win)
        {
            string title = win? Strings.YouWonTitle : Strings.YouLostTitle;
            string message = win? Strings.WinMSG : Strings.LoseMSG;
            Shell.Current.ShowPopup(new GameOverPopUp(title, message));
        }
        private void OnTimeLeftChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(TimeLeft));
        }
        private void OnUiChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Board));
            OnPropertyChanged(nameof(DiscardTileSource));
        }
        private void OnGameChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(OtherPlayerName1));
            OnPropertyChanged(nameof(OtherPlayerName2));
            OnPropertyChanged(nameof(OtherPlayerName3));
            OnPropertyChanged(nameof(IsMyTurn));
            OnPropertyChanged(nameof(IsPlayer1Turn));
            OnPropertyChanged(nameof(IsPlayer2Turn));
            OnPropertyChanged(nameof(IsPlayer3Turn));
            (DiscardSelectedCommand as Command)?.ChangeCanExecute();
            (TakeDiscardCommand as Command)?.ChangeCanExecute();
        }
        private void OnJoinComplete(Task task)
        {
            if (!task.IsCompletedSuccessfully)
                Toast.Make(Strings.JoinGameErr, ToastDuration.Long).Show();
        }
        public void AddSnapshotListener() => game.AddSnapshotListener();
        public void RemoveSnapshotListener() => game.RemoveSnapshotListener();
    }
}
