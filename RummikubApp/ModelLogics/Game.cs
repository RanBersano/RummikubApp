using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.CloudFirestore;
using RummikubApp.Models;
namespace RummikubApp.ModelLogics
{
    public class Game : GameModel
    {
        public Game(GameSize selectedGameSize)
        {
            RegisterTimer();
            HostName = new User().UserName;
            Players = selectedGameSize.Size;
            Created = DateTime.Now;
            CurrentNumOfPlayers = 1;

            Deck = new Deck();
            DeckData = Deck.ExportToArray();

            // יד למארח: 14 ואז נרחיב ל-18 עם סלוטים ריקים דרך Board
            TileData[] first14 = Deck.DealTiles(14);
            Board hostBoard = new Board(first14);
            hostBoard.EnsureCapacity();
            HostHand = hostBoard.ExportToArray();

            DeckData = Deck.ExportToArray();
        }

        public Game()
        {
            RegisterTimer();
        }
        private void RegisterTimer()
        {
            WeakReferenceMessenger.Default.Register<AppMessage<long>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });
        }

        private void OnMessageReceived(long timeLeft)
        {
            TimeLeft = timeLeft == Keys.FinishedSignal ? Strings.TimeUp : double.Round(timeLeft / 1000, 1).ToString();
            TimeLeftChanged?.Invoke(this, EventArgs.Empty);
        }
        [Plugin.CloudFirestore.Attributes.Ignored] private Board? _myBoardCache;
        private Board GetMyBoard()
        {
            if (_myBoardCache == null)
            {
                TileData[] hand = GetMyHand();
                _myBoardCache = new Board(hand);
                _myBoardCache.EnsureCapacity();
            }
            return _myBoardCache;
        }
        private void RebuildDeckFromData()
        {
            Deck = new Deck(DeckData);
        }

        public override string GetOtherPlayerName(int index)
        {
            string[] others = new string[]
            {
                HostName, PlayerName2, PlayerName3, PlayerName4
            };

            // בלי List: בונים מערך זמני של עד 3
            string[] valid = new string[3];
            int count = 0;

            for (int i = 0; i < others.Length; i++)
            {
                string name = others[i];
                if (!string.IsNullOrWhiteSpace(name) && !string.Equals(name, MyName, StringComparison.Ordinal))
                {
                    if (count < valid.Length)
                    {
                        valid[count] = name;
                        count++;
                    }
                }
            }

            if (index >= 0 && index < count)
            {
                return valid[index];
            }
            return string.Empty;
        }

        public override void SetDocument(Action<Task> onComplete)
        {
            Id = fbd.SetDocument(this, Keys.GamesCollection, Id, onComplete);
        }

        public override void AddSnapshotListener()
        {
            ilr = fbd.AddSnapshotListener(Keys.GamesCollection, Id, OnChange);
        }

        public override void RemoveSnapshotListener()
        {
            ilr?.Remove();
            DeleteDocument(OnComplete);
        }

        public override void DeleteDocument(Action<Task> onComplete)
        {
            fbd.DeleteDocument(Keys.GamesCollection, Id, onComplete);
        }

        public override void MoveToNextTurn(Action<Task> onComplete)
        {
            int next = CurrentTurnIndex + 1;
            if (next > Players)
            {
                next = 1;
            }

            CurrentTurnIndex = next;
            HasDrawnThisTurn = false;
            Dictionary<string, object> updates = new Dictionary<string, object>();
            updates[nameof(CurrentTurnIndex)] = CurrentTurnIndex;

            fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
        }

        public override void UpdateGuestUser(Action<Task> onComplete)
        {
            if (IsFull)
            {
                Task failed = Task.FromException(new InvalidOperationException(Strings.GameFull));
                onComplete(failed);
                return;
            }

            if (Deck == null)
            {
                RebuildDeckFromData();
            }

            Dictionary<string, object> updates = new Dictionary<string, object>();
            bool handDealt = false;

            if (string.IsNullOrEmpty(PlayerName2))
            {
                PlayerName2 = MyName;
                updates[nameof(PlayerName2)] = PlayerName2;

                if (Player2Hand == null || Player2Hand.Length == 0)
                {
                    TileData[] first14 = Deck!.DealTiles(14);
                    Board b = new Board(first14);
                    b.EnsureCapacity();
                    Player2Hand = b.ExportToArray();

                    updates[nameof(Player2Hand)] = Player2Hand;
                    handDealt = true;
                }
            }
            else if (Players >= 3 && string.IsNullOrEmpty(PlayerName3))
            {
                PlayerName3 = MyName;
                updates[nameof(PlayerName3)] = PlayerName3;

                if (Player3Hand == null || Player3Hand.Length == 0)
                {
                    TileData[] first14 = Deck!.DealTiles(14);
                    Board b = new Board(first14);
                    b.EnsureCapacity();
                    Player3Hand = b.ExportToArray();

                    updates[nameof(Player3Hand)] = Player3Hand;
                    handDealt = true;
                }
            }
            else if (Players == 4 && string.IsNullOrEmpty(PlayerName4))
            {
                PlayerName4 = MyName;
                updates[nameof(PlayerName4)] = PlayerName4;

                if (Player4Hand == null || Player4Hand.Length == 0)
                {
                    TileData[] first14 = Deck!.DealTiles(14);
                    Board b = new Board(first14);
                    b.EnsureCapacity();
                    Player4Hand = b.ExportToArray();

                    updates[nameof(Player4Hand)] = Player4Hand;
                    handDealt = true;
                }
            }
            else
            {
                Task failed = Task.FromException(new InvalidOperationException(Strings.GameFull));
                onComplete(failed);
                return;
            }

            CurrentNumOfPlayers++;
            updates[nameof(CurrentNumOfPlayers)] = CurrentNumOfPlayers;

            if (CurrentNumOfPlayers >= Players)
            {
                IsFull = true;
                updates[nameof(IsFull)] = true;
            }

            if (handDealt)
            {
                DeckData = Deck!.ExportToArray();
                updates[nameof(DeckData)] = DeckData;
            }

            fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
        }

        private TileData[] GetHandForPlayer(string playerName)
        {
            if (playerName == HostName) return HostHand;
            if (playerName == PlayerName2) return Player2Hand;
            if (playerName == PlayerName3) return Player3Hand;
            if (playerName == PlayerName4) return Player4Hand;
            return new TileData[0];
        }

        private void SetHandForPlayer(string playerName, TileData[] hand)
        {
            if (playerName == HostName) { HostHand = hand; return; }
            if (playerName == PlayerName2) { Player2Hand = hand; return; }
            if (playerName == PlayerName3) { Player3Hand = hand; return; }
            if (playerName == PlayerName4) { Player4Hand = hand; return; }
        }

        private string GetHandFieldNameForPlayer(string playerName)
        {
            if (playerName == HostName) return nameof(HostHand);
            if (playerName == PlayerName2) return nameof(Player2Hand);
            if (playerName == PlayerName3) return nameof(Player3Hand);
            if (playerName == PlayerName4) return nameof(Player4Hand);
            return string.Empty;
        }

        public override void HandleTileTap(int tappedIndex, Action<Task> onComplete)
        {
            Board board = GetMyBoard();

            int before = board.SelectedIndex;
            bool changed = board.HandleTap(tappedIndex);
            int after = board.SelectedIndex;

            if (!changed)
            {
                onComplete(Task.CompletedTask);
                return;
            }

            // אם הייתה החלפה (כלומר היה before != -1 וה־after חזר -1 והאינדקס שונה)
            bool swapped = (before != -1 && after == -1 && before != tappedIndex);

            // רק אם היה SWAP שומרים ל-Firestore
            if (swapped)
            {
                TileData[] newHand = board.ExportToArray();

                // עדכון היד שלי בתוך ה-Game עצמו
                SetHandForPlayer(MyName, newHand);

                Dictionary<string, object> updates = new Dictionary<string, object>();
                string field = GetHandFieldNameForPlayer(MyName);
                updates[field] = newHand;

                fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
                return;
            }

            // אם זה רק שינוי בחירה (סימון/ביטול) — לא שומרים בפיירסטור
            onComplete(Task.CompletedTask);
        }


        public override void TakeTileFromDeckAndSave(Action<Task> onComplete)
        {
            if (HasDrawnThisTurn) { onComplete(Task.CompletedTask); return; }

            if (Deck == null)
            {
                RebuildDeckFromData();
            }

            TileData? drawn = Deck?.DrawTileData();
            if (drawn == null)
            {
                onComplete(Task.CompletedTask);
                return;
            }

            TileData[] hand = GetHandForPlayer(MyName);
            Board board = new Board(hand);

            bool placed = board.PlaceTileInFirstEmpty(drawn);
            if (!placed)
            {
                onComplete(Task.CompletedTask);
                return;
            }

            TileData[] newHand = board.ExportToArray();
            SetHandForPlayer(MyName, newHand);

            DeckData = Deck!.ExportToArray();
            HasDrawnThisTurn = true;
            Dictionary<string, object> updates = new Dictionary<string, object>();
            updates[nameof(DeckData)] = DeckData;

            string handField = GetHandFieldNameForPlayer(MyName);
            updates[handField] = newHand;

            fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
        }

        public override void InitGrid(Grid deck)
        {
            IndexedButton btn;

            for (int i = 0; i < 4; i++)
            {
                deck.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                deck.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    btn = new IndexedButton(i, j)
                    {
                        BackgroundColor = Color.FromArgb("#C8BFB1")
                    };
                    btn.Clicked += OnButtonClicked;
                    deck.Add(btn, j, i);
                }
            }
        }

        protected override void OnButtonClicked(object? sender, EventArgs e)
        {
            if (!IsFull) return;
            if (CurrentPlayerName != MyName) return;

            IndexedButton? btn = sender as IndexedButton;
            if (btn == null) return;

            TakeTileFromDeckAndSave(OnTakeTileComplete);
        }

        private void OnTakeTileComplete(Task task)
        {
            if (!task.IsCompletedSuccessfully)
            {
                return;
            }
        }

        private void OnCompleteTurn(Task task)
        {
        }

        protected override void OnComplete(Task task)
        {
            if (task.IsCompletedSuccessfully)
            {
                OnGameDeleted?.Invoke(this, EventArgs.Empty);
            }
        }
        [Plugin.CloudFirestore.Attributes.Ignored] private bool _startTimerWasTriggered = false;    
        protected override void OnChange(IDocumentSnapshot? snapshot, Exception? error)
        {
            Game? updated = snapshot?.ToObject<Game>();
            if (updated != null)
            {
                Players = updated.Players;
                IsFull = updated.IsFull;
                CurrentNumOfPlayers = updated.CurrentNumOfPlayers;
                HostName = updated.HostName;
                PlayerName2 = updated.PlayerName2;
                PlayerName3 = updated.PlayerName3;
                PlayerName4 = updated.PlayerName4;
                CurrentTurnIndex = updated.CurrentTurnIndex;

                DeckData = updated.DeckData ?? new TileData[0];
                HostHand = updated.HostHand ?? new TileData[0];
                Player2Hand = updated.Player2Hand ?? new TileData[0];
                Player3Hand = updated.Player3Hand ?? new TileData[0];
                Player4Hand = updated.Player4Hand ?? new TileData[0];
                DiscardTile = updated.DiscardTile ?? new TileData { IsPresent = false };
                _myBoardCache = null;
                RebuildDeckFromData();  
                OnGameChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Shell.Current.Navigation.PopAsync();
                    Toast.Make(Strings.GameDeleted, ToastDuration.Long).Show();
                });
            }
            if (IsFull && !_startTimerWasTriggered)
            {
                _startTimerWasTriggered = true;
                WeakReferenceMessenger.Default.Send(new AppMessage<TimerSettings>(timerSettings));
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new AppMessage<bool>(true));
                TimeLeft = string.Empty;
                TimeLeftChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public TileData[] GetMyHand()
        {
            return GetHandForPlayer(MyName);
        }
        public override void DiscardSelectedTileAndSave(int selectedIndex, Action<Task> onComplete)
        {
            if (!IsFull) { onComplete(Task.CompletedTask); return; }
            if (CurrentPlayerName != MyName) { onComplete(Task.CompletedTask); return; }
            if (!HasDrawnThisTurn) { onComplete(Task.CompletedTask); return; }
            // לוקחים את היד הנוכחית שלי
            TileData[] hand = GetMyHand();
            Board board = new Board(hand);
            board.EnsureCapacity();

            if (selectedIndex < 0 || selectedIndex >= board.Tiles.Length)
            {
                onComplete(Task.CompletedTask);
                return;
            }

            TileData chosen = board.Tiles[selectedIndex];
            if (chosen == null || chosen.IsEmptySlot)
            {
                onComplete(Task.CompletedTask);
                return;
            }

            // שמים את האריח שנזרק ב-DiscardTile
            DiscardTile = new TileData
            {
                Color = chosen.Color,
                Number = chosen.Number,
                IsJoker = chosen.IsJoker,
                IsEmptySlot = false,
                IsPresent = true
            };

            // מורידים אותו מהיד (הופכים לריק)
            board.Tiles[selectedIndex] = new TileData
            {
                Color = 0,
                Number = 0,
                IsJoker = false,
                IsEmptySlot = true,
                IsPresent = false
            };

            TileData[] newHand = board.ExportToArray();
            SetHandForPlayer(MyName, newHand);

            // מעבירים תור לשחקן הבא
            int next = CurrentTurnIndex + 1;
            if (next > Players) next = 1;
            CurrentTurnIndex = next;
            HasDrawnThisTurn = false;
            Dictionary<string, object> updates = new Dictionary<string, object>();
            updates[GetHandFieldNameForPlayer(MyName)] = newHand;
            updates[nameof(DiscardTile)] = DiscardTile;
            updates[nameof(CurrentTurnIndex)] = CurrentTurnIndex;

            fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
        }

        public override void TakeDiscardAndSave(Action<Task> onComplete)
        {
            if (!IsFull) { onComplete(Task.CompletedTask); return; }
            if (CurrentPlayerName != MyName) { onComplete(Task.CompletedTask); return; }
            if (HasDrawnThisTurn) { onComplete(Task.CompletedTask); return; }
            if (DiscardTile == null || !DiscardTile.IsPresent) { onComplete(Task.CompletedTask); return; }

            // מוסיפים ליד שלי בסלוט הריק הראשון
            TileData[] hand = GetMyHand();
            Board board = new Board(hand);
            board.EnsureCapacity();

            TileData tileToAdd = new TileData
            {
                Color = DiscardTile.Color,
                Number = DiscardTile.Number,
                IsJoker = DiscardTile.IsJoker,
                IsEmptySlot = false,
                IsPresent = false
            };

            bool placed = board.PlaceTileInFirstEmpty(tileToAdd);
            if (!placed) { onComplete(Task.CompletedTask); return; }

            TileData[] newHand = board.ExportToArray();
            SetHandForPlayer(MyName, newHand);

            // מאפסים את ה-Discard (כי לקחת אותו)
            DiscardTile = new TileData { IsPresent = false };
            HasDrawnThisTurn = true;
            Dictionary<string, object> updates = new Dictionary<string, object>();
            updates[GetHandFieldNameForPlayer(MyName)] = newHand;
            updates[nameof(DiscardTile)] = DiscardTile;

            fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
        }

    }
}