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

        public string MyName => game.MyName;

        public string OtherPlayerName1 => game.GetOtherPlayerName(0);
        public string OtherPlayerName2 => game.GetOtherPlayerName(1);
        public string OtherPlayerName3 => game.GetOtherPlayerName(2);

        public bool IsMyTurn => game.IsFull && game.CurrentPlayerName == game.MyName;

        public bool IsPlayer1Turn => game.CurrentPlayerName == game.GetOtherPlayerName(0);

        public bool IsPlayer2Turn => game.CurrentPlayerName == game.GetOtherPlayerName(1);

        public bool IsPlayer3Turn => game.CurrentPlayerName == game.GetOtherPlayerName(2);

        public ICommand TileTappedCommand { get; }
        public ICommand MoveCommand { get; }

        public GamePageVM(Game game, Grid deckGrid)
        {
            this.game = game;

            game.OnGameChanged += OnGameChanged;
            game.InitGrid(deckGrid);

            TileTappedCommand = new Command<Tile>(OnTileTapped);

            MoveCommand = new Command(
                () => game.MoveToNextTurn(_ => { }),
                () => IsMyTurn
            );

            LoadBoardFromGame();

            if (!game.IsHostUser)
            {
                game.UpdateGuestUser(OnJoinComplete);
            }
        }

        // =========================
        // טעינת הלוח מה-Game
        // =========================
        private void LoadBoardFromGame()
        {
            Board.Clear();

            TileData[] hand = game.GetMyHand();

            for (int i = 0; i < hand.Length; i++)
            {
                Board.Add(CreateTileFromData(hand[i]));
            }
        }

        // =========================
        // לחיצה על אריח
        // =========================
        private void OnTileTapped(Tile tappedTile)
        {
            int index = Board.IndexOf(tappedTile);
            if (index < 0)
            {
                return;
            }

            game.HandleTileTap(index, _ => { });
        }

        // =========================
        // ריענון מה-Firestore
        // =========================
        private void OnGameChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(OtherPlayerName1));
            OnPropertyChanged(nameof(OtherPlayerName2));
            OnPropertyChanged(nameof(OtherPlayerName3));

            OnPropertyChanged(nameof(IsMyTurn));
            OnPropertyChanged(nameof(IsPlayer1Turn));
            OnPropertyChanged(nameof(IsPlayer2Turn));
            OnPropertyChanged(nameof(IsPlayer3Turn));

            LoadBoardFromGame();

            (MoveCommand as Command)?.ChangeCanExecute();
        }

        private void OnJoinComplete(Task task)
        {
            if (!task.IsCompletedSuccessfully)
            {
                Toast.Make(Strings.JoinGameErr, ToastDuration.Long).Show();
            }
        }

        // =========================
        // TileData → Tile (UI בלבד)
        // =========================
        private Tile CreateTileFromData(TileData data)
        {
            if (data.IsEmptySlot)
            {
                return Tile.CreateEmptySlot();
            }

            if (data.IsJoker)
            {
                return new Tile();
            }

            return new Tile(
                (TileModel.Colors)data.Color,
                data.Number
            );
        }
        public void AddSnapshotListener()
{
    game.AddSnapshotListener();
}

public void RemoveSnapshotListener()
{
    game.RemoveSnapshotListener();
}
    }
}