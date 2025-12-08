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
        private readonly Deck deck;
        private readonly Player me;
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
        private void OnTileTapped(Tile tappedTile)
        {
            int index = Board.IndexOf(tappedTile);
            if (index < 0)
                return;
            // אם עוד לא נבחר אריח – זה האריח הנבחר הראשון
            if (selectedIndex == -1)
            {
                selectedIndex = index;
                tappedTile.IsSelected = true;
                OnPropertyChanged(nameof(Board));
                return;
            }
            // אם לוחצים שוב על אותו אריח – ביטול בחירה
            if (selectedIndex == index)
            {
                tappedTile.IsSelected = false;
                selectedIndex = -1;
                OnPropertyChanged(nameof(Board));
                return;
            }
            // אם נבחר כבר אחד ועכשיו לוחצים על אריח אחר – מחליפים ביניהם
            Tile firstTile = Board[selectedIndex];
            Tile secondTile = Board[index];
            firstTile.IsSelected = false;
            Board[selectedIndex] = secondTile;
            Board[index] = firstTile;
            selectedIndex = -1;
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
            deck = new Deck();
            me = new Player(MyName);
            for (int i = 0; i < 14; i++)
            {
                me.DrawFromDeck(deck);
            }
            foreach (Tile tile in me.Board.Tiles)
            {
                Board.Add(tile);
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
