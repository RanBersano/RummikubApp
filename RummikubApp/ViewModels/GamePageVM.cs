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
        private readonly Command<Tile> tileTappedCommand;
        private int selectedIndex = -1;
        public string MyName => game.MyName;
        public string OtherPlayerName1 => game.GetOtherPlayerName(0);
        public string OtherPlayerName2 => game.GetOtherPlayerName(1);
        public string OtherPlayerName3 => game.GetOtherPlayerName(2);
        public bool IsMyTurn => game.IsFull && game.CurrentPlayerName == game.MyName;
        public bool IsPlayer1Turn => game.CurrentPlayerName == game.GetOtherPlayerName(0);
        public bool IsPlayer2Turn => game.CurrentPlayerName == game.GetOtherPlayerName(1);
        public bool IsPlayer3Turn => game.CurrentPlayerName == game.GetOtherPlayerName(2);
        public ObservableCollection<Tile> Board { get; set; } = new ObservableCollection<Tile>();
        private Command? _moveCommand;
        public ICommand MoveCommand => _moveCommand ??= new Command(
            () =>
            {
                if (!IsMyTurn)
                    return;
                game.MoveToNextTurn(OnCompleteMove);
            },
            () => IsMyTurn
        );
        public ICommand TileTappedCommand => tileTappedCommand;
        private Tile CreateTileFromData(TileData data)
        {
            if (data.IsJoker)
            {
                Tile joker = new Tile();
                return joker;
            }

            TileModel.Colors color = (TileModel.Colors)data.Color;
            int number = data.Number;

            Tile tile = new Tile(color, number);
            return tile;
        }
        private void OnTileTapped(Tile tappedTile)
        {
            int index = Board.IndexOf(tappedTile);
            if (index < 0)
            {
                return;
            }

            game.HandleTileTap(index, OnHandSaved);
        }

        private void OnHandSaved(Task task)
        {
            // אפשר Toast אם יש שגיאה
        }
        private List<TileData> BuildHandDataFromBoard()
        {
            List<TileData> list = new List<TileData>();

            for (int i = 0; i < Board.Count; i++)
            {
                Tile tile = Board[i];

                TileData data = new TileData();

                if (tile.IsEmptySlot)
                {
                    data.IsEmptySlot = true;
                    data.IsJoker = false;
                    data.Color = 0;
                    data.Number = 0;
                }
                else if (tile.IsJoker)
                {
                    data.IsEmptySlot = false;
                    data.IsJoker = true;
                    data.Color = 0;
                    data.Number = 0;
                }
                else
                {
                    data.IsEmptySlot = false;
                    data.IsJoker = false;
                    data.Color = (int)tile.Color;
                    data.Number = tile.Number;
                }

                list.Add(data);
            }

            return list;
        }
        private void OnCompleteMove(Task t)
        {
            // אפשר להראות Toast במקרה של שגיאה אם תרצה
        }
        public GamePageVM(Game game, Grid deckGrid)
        {
            this.game = game;
            game.OnGameChanged += OnGameChanged;
            game.InitGrid(deckGrid);
            Board.Clear();
            List<TileData> myHandData = game.GetHandDataForPlayer(MyName);
            for (int i = 0; i < myHandData.Count; i++)
            {
                Tile tile = CreateTileFromData(myHandData[i]);
                if (myHandData[i].IsEmptySlot)
                {
                    tile.IsEmptySlot = true;
                    tile.Source = null;
                    tile.IsJoker = false;
                    tile.Number = 0;
                }
                Board.Add(tile);
            }
            while (Board.Count < 18)
            {
                Tile emptySlot = Tile.CreateEmptySlot();
                Board.Add(emptySlot);
            }
            tileTappedCommand = new Command<Tile>(OnTileTapped);
            if (!game.IsHostUser)
            {
                game.UpdateGuestUser(OnComplete);
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

            // ריענון היד שלי מהפיירסטור
            Board.Clear();
            List<TileData> myHandData = game.GetHandDataForPlayer(MyName);
            for (int i = 0; i < myHandData.Count; i++)
            {
                Tile tile = CreateTileFromData(myHandData[i]);
                Board.Add(tile);
            }
            while (Board.Count < 18)
            {
                Tile emptySlot = Tile.CreateEmptySlot();
                Board.Add(emptySlot);
            }
            _moveCommand?.ChangeCanExecute();
        }
        private void OnComplete(Task task)
        {
            if (!task.IsCompletedSuccessfully)
                Toast.Make(Strings.JoinGameErr, ToastDuration.Long).Show();
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
