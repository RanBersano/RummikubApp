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

        private readonly Command<int> tileTappedCommand;
        public ICommand TileTappedCommand => tileTappedCommand;
        public ICommand MoveCommand { get; }
        public GamePageVM(Game game, Grid deckGrid)
        {
            this.game = game;

            game.OnGameChanged += OnGameChanged;
            game.InitGrid(deckGrid);

            tileTappedCommand = new Command<int>(OnTileTapped);
            RefreshBoardFromGame_NoClear();
            MoveCommand = new Command(
                () => game.MoveToNextTurn(_ => { }),
                () => IsMyTurn
            );
            if (!game.IsHostUser)
            {
                game.UpdateGuestUser(OnJoinComplete);
            }
        }

        private int _selectedIndex = -1;

        private void OnTileTapped(int index)
        {
            if (index < 0 || index >= Board.Count)
                return;

            int prev = _selectedIndex;

            game.HandleTileTap(index, _ =>
            {
                // 1) נרענן את המערך מהמשחק (כולל swap אם היה)
                RefreshBoardFromGame_NoClear();

                // 2) לוגיקת selection UI (לא קשורה ל-firestore)
                if (prev == -1)
                {
                    // בחירה ראשונה - לא לבחור ריק
                    if (!Board[index].IsEmptySlot)
                    {
                        _selectedIndex = index;
                        Board[index].IsSelected = true;
                    }
                    return;
                }

                // אם לחץ על אותו אריח - ביטול
                if (prev == index)
                {
                    Board[prev].IsSelected = false;
                    _selectedIndex = -1;
                    return;
                }

                // אם לחץ על משהו אחר - swap ואז אין בחירה
                if (prev != -1)
                {
                    Board[prev].IsSelected = false;
                    _selectedIndex = -1;
                }
            });
        }
        private void OnHandSaved(Task task)
        {
            if (task.IsCompletedSuccessfully)
                RefreshBoardFromGame_NoClear();
        }



        private void RefreshBoardFromGame_NoClear()
        {
            TileData[] hand = game.GetMyHand();

            // אם צריך לבנות מחדש רק פעם ראשונה
            if (Board.Count != hand.Length)
            {
                Board.Clear();
                for (int i = 0; i < hand.Length; i++)
                {
                    Tile t = CreateTileFromData(hand[i]);
                    t.Index = i;
                    t.IsSelected = (i == _selectedIndex);
                    Board.Add(t);
                }
                return;
            }

            // עדכון בלי "העלמות"
            for (int i = 0; i < hand.Length; i++)
            {
                Tile t = CreateTileFromData(hand[i]);
                t.Index = i;
                t.IsSelected = (i == _selectedIndex);
                Board[i] = t;
            }
        }

        // ריענון מה-Firestore
        private void OnGameChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(OtherPlayerName1));
            OnPropertyChanged(nameof(OtherPlayerName2));
            OnPropertyChanged(nameof(OtherPlayerName3));
            OnPropertyChanged(nameof(IsMyTurn));
            OnPropertyChanged(nameof(IsPlayer1Turn));
            OnPropertyChanged(nameof(IsPlayer2Turn));
            OnPropertyChanged(nameof(IsPlayer3Turn));
            RefreshBoardFromGame_NoClear();
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