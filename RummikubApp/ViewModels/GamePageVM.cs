using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using RummikubApp.ModelLogics;
using RummikubApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
namespace RummikubApp.ViewModels
{
    public class GamePageVM : ObservableObject
    {
        private readonly Game game;
        public ObservableCollection<Tile> Board { get; } = new ObservableCollection<Tile>();
        public string TimeLeft => game.TimeLeft;
        public string MyName => game.MyName;
        public string OtherPlayerName1 => game.GetOtherPlayerName(0);
        public string OtherPlayerName2 => game.GetOtherPlayerName(1);
        public string OtherPlayerName3 => game.GetOtherPlayerName(2);
        public bool IsMyTurn => game.IsFull && game.CurrentPlayerName == game.MyName;
        public bool IsPlayer1Turn => game.CurrentPlayerName == game.GetOtherPlayerName(0);
        public bool IsPlayer2Turn => game.CurrentPlayerName == game.GetOtherPlayerName(1);
        public bool IsPlayer3Turn => game.CurrentPlayerName == game.GetOtherPlayerName(2);
        public bool CanTakeTile => IsMyTurn && !game.HasDrawnThisTurn;
        public bool CanDiscard => IsMyTurn && game.HasDrawnThisTurn;
        private readonly Command<int> tileTappedCommand;
        public ICommand TileTappedCommand => tileTappedCommand;
        public ICommand TakeDiscardCommand { get; }
        public ICommand DiscardSelectedCommand { get; }
        private int _selectedIndex = -1;
        private ImageSource? discardTileSource;
        public ImageSource? DiscardTileSource
        {
            get => discardTileSource;
            set
            {
                discardTileSource = value;
                OnPropertyChanged();
            }
        }
        public GamePageVM(Game game, Grid deckGrid)
        {
            this.game = game;
            game.OnGameChanged += OnGameChanged;
            game.InitGrid(deckGrid);
            game.TimeLeftChanged += OnTimeLeftChanged;
            tileTappedCommand = new Command<int>(OnTileTapped);
            TakeDiscardCommand = new Command(
                () => game.TakeDiscardAndSave(_ => { }),
                () => IsMyTurn && game.DiscardTile != null && game.DiscardTile.IsPresent && CanTakeTile);
            DiscardSelectedCommand = new Command(
                () =>
                {
                    if (!IsMyTurn) return;
                    if (_selectedIndex < 0) return;
                    int idx = _selectedIndex;
                    _selectedIndex = -1;
                    game.DiscardSelectedTileAndSave(idx, _ => { });
                },
                () => IsMyTurn && _selectedIndex >= 0);
            if (!game.IsHostUser)
                game.UpdateGuestUser(OnJoinComplete);
            RefreshBoardFromGame_NoClear();
            UpdateDiscardTile();
        }
        private void OnTimeLeftChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(TimeLeft));
        }   
        private void OnTileTapped(int index)
        {
            if (index < 0 || index >= Board.Count)
                return;
            if (_selectedIndex == -1 && Board[index].IsEmptyTile)
                return;
            if (_selectedIndex == index)
            {
                _selectedIndex = -1;
                RefreshBoardFromGame_NoClear();
                return;
            }
            if (_selectedIndex != -1)
            {
                int first = _selectedIndex;
                int second = index;
                _selectedIndex = -1;
                game.HandleTileTap(first, _ => { });
                game.HandleTileTap(second, _ => { });
                RefreshBoardFromGame_NoClear();
                return;
            }
            _selectedIndex = index;
            RefreshBoardFromGame_NoClear();
            (DiscardSelectedCommand as Command)?.ChangeCanExecute();
        }
        private void RefreshBoardFromGame_NoClear()
        {
            TileData[] hand = game.GetMyHand();
            if (Board.Count != hand.Length)
            {
                Board.Clear();
                for (int i = 0; i < hand.Length; i++)
                {
                    Tile t = Tile.FromTileData(hand[i]);
                    t.Index = i;
                    t.IsSelected = (i == _selectedIndex);
                    Board.Add(t);
                }
                return;
            }
            for (int i = 0; i < hand.Length; i++)
            {
                Tile t = Tile.FromTileData(hand[i]);
                t.Index = i;
                t.IsSelected = (i == _selectedIndex);
                Board[i] = t;
            }
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
            OnPropertyChanged(nameof(CanTakeTile));
            OnPropertyChanged(nameof(CanDiscard));
            RefreshBoardFromGame_NoClear();
            UpdateDiscardTile();
            (DiscardSelectedCommand as Command)?.ChangeCanExecute();
            (TakeDiscardCommand as Command)?.ChangeCanExecute();
            (TakeDiscardCommand as Command)?.ChangeCanExecute();
            (DiscardSelectedCommand as Command)?.ChangeCanExecute();
        }
        private void UpdateDiscardTile()
        {
            TileData discarded = game.DiscardTile;
            if (discarded == null || !discarded.IsPresent)
            {
                DiscardTileSource = null;
                return;
            }
            if (discarded.IsJoker)
            {
                DiscardTileSource = Strings.Joker;
                return;
            }
            Tile t = new Tile((TileModel.Colors)discarded.Color, discarded.Number);
            DiscardTileSource = t.Source;
        }
        private ImageSource? TileDataToImageSource(TileData data)
        {
            if (data == null) return null;
            if (!data.IsPresent) return null;
            if (data.IsEmptyTile) return null;
            if (data.IsJoker)
                return ImageSource.FromFile(Strings.Joker);
            return Tile.GetSourceFor((TileModel.Colors)data.Color, data.Number);
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
