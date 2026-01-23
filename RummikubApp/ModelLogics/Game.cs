using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.CloudFirestore;
using RummikubApp.Models;
namespace RummikubApp.ModelLogics
{
    public class Game : GameModel
    {
        private readonly Tile tileFactory = new Tile();

        public Game(GameSize selectedGameSize)
        {
            RegisterTimer();
            HostName = new User().UserName;
            Players = selectedGameSize.Size;
            Created = DateTime.Now;
            CurrentNumOfPlayers = 1;
            Deck = new Deck();
            DeckData = Deck.ExportToArray();
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
        protected override void RegisterTimer()
        {
            WeakReferenceMessenger.Default.Register<AppMessage<long>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });
        }
        protected override void OnMessageReceived(long timeLeft)
        {
            TimeLeft = timeLeft == Keys.FinishedSignal ? Strings.TimeUp : double.Round(timeLeft / 1000, 1).ToString();
            TimeLeftChanged?.Invoke(this, EventArgs.Empty);
        }
        protected override Board GetMyBoard()
        {
            if (MyBoardCache == null)
            {
                TileData[] hand = GetMyHand();
                MyBoardCache = new Board(hand);
                MyBoardCache.EnsureCapacity();
            }
            return MyBoardCache;
        }
        protected override void RebuildDeckFromData()
        {
            Deck = new Deck(DeckData);
        }
        public override string GetOtherPlayerName(int index)
        {
            string[] others = new string[]
            {
                HostName, PlayerName2, PlayerName3, PlayerName4
            };
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
                return valid[index];
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
                next = 1;
            CurrentTurnIndex = next;
            HasDrawnThisTurn = false;
            Dictionary<string, object> updates = new Dictionary<string, object>();
            updates[nameof(HasDrawnThisTurn)] = HasDrawnThisTurn;
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
                RebuildDeckFromData();
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
        protected override TileData[] GetHandForPlayer(string playerName)
        {
            if (playerName == HostName) 
                return HostHand;
            if (playerName == PlayerName2) 
                return Player2Hand;
            if (playerName == PlayerName3) 
                return Player3Hand;
            if (playerName == PlayerName4) 
                return Player4Hand;
            return [];
        }
        protected override void SetHandForPlayer(string playerName, TileData[] hand)
        {
            if (playerName == HostName) 
            { 
                HostHand = hand; 
                return; 
            }
            if (playerName == PlayerName2) 
            { 
                Player2Hand = hand; 
                return; 
            }
            if (playerName == PlayerName3) 
            { 
                Player3Hand = hand; 
                return; 
            }
            if (playerName == PlayerName4) 
            { Player4Hand = hand; 
                return; 
            }
        }
        protected override string GetHandFieldNameForPlayer(string playerName)
        {
            if (playerName == HostName) 
                return nameof(HostHand);
            if (playerName == PlayerName2) 
                return nameof(Player2Hand);
            if (playerName == PlayerName3) 
                return nameof(Player3Hand);
            if (playerName == PlayerName4) 
                return nameof(Player4Hand);
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
            bool swapped = (before != -1 && after == -1 && before != tappedIndex);
            if (swapped)
            {
                TileData[] newHand = board.ExportToArray();
                SetHandForPlayer(MyName, newHand);
                Dictionary<string, object> updates = new Dictionary<string, object>();
                string field = GetHandFieldNameForPlayer(MyName);
                updates[field] = newHand;
                fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
                return;
            }
            onComplete(Task.CompletedTask);
        }
        public override void TakeTileFromDeckAndSave(Action<Task> onComplete)
        {
            if (HasDrawnThisTurn) 
            { 
                onComplete(Task.CompletedTask); 
                return; 
            }
            if (Deck == null)
                RebuildDeckFromData();
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
            updates[nameof(HasDrawnThisTurn)] = HasDrawnThisTurn;

            string handField = GetHandFieldNameForPlayer(MyName);
            updates[handField] = newHand;

            fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
        }
        public override void InitGrid(Grid deck)
        {
            IndexedButton btn;
            for (int i = 0; i < 3; i++)
            {
                deck.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                deck.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    btn = new IndexedButton(i, j)
                    {
                        BackgroundColor = Color.FromArgb("#FFF6D6")
                    };
                    btn.Clicked += OnButtonClicked;
                    deck.Add(btn, j, i);
                }
            }
        }
        protected override void OnButtonClicked(object? sender, EventArgs e)
        {
            if (!IsFull) 
                return;
            if (CurrentPlayerName != MyName)
                return;
            IndexedButton? btn = sender as IndexedButton;
            if (btn == null)
                return;
            TakeTileFromDeckAndSave(OnTakeTileComplete);
        }
        protected override void OnTakeTileComplete(Task task)
        {
            if (!task.IsCompletedSuccessfully)
                return;
        }
        protected override void OnComplete(Task task)
        {
            if (task.IsCompletedSuccessfully)
            {
                OnGameDeleted?.Invoke(this, EventArgs.Empty);
            }
        }  
        protected override void OnChange(IDocumentSnapshot? snapshot, Exception? error)
        {
            Game? updated = snapshot?.ToObject<Game>();
            if (updated != null)
            {
                if(updated.IsGameOver && !IsGameOver)
                    GameOver?.Invoke(this, false);
                HasDrawnThisTurn = updated.HasDrawnThisTurn;
                IsGameOver = updated.IsGameOver;
                WinnerIndex = updated.WinnerIndex;
                Players = updated.Players;
                IsFull = updated.IsFull;
                CurrentNumOfPlayers = updated.CurrentNumOfPlayers;
                HostName = updated.HostName;
                PlayerName2 = updated.PlayerName2;
                PlayerName3 = updated.PlayerName3;
                PlayerName4 = updated.PlayerName4;
                CurrentTurnIndex = updated.CurrentTurnIndex;
                DeckData = updated.DeckData ?? [];
                HostHand = updated.HostHand ?? [];
                Player2Hand = updated.Player2Hand ?? [];
                Player3Hand = updated.Player3Hand ?? [];
                Player4Hand = updated.Player4Hand ?? [];
                DiscardTile = updated.DiscardTile ?? new TileData { IsPresent = false };
                MyBoardCache = null;
                RebuildDeckFromData();
                RefreshUi();
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
            if (IsFull && !StartTimerWasTriggered)
            {
                StartTimerWasTriggered = true;
                WeakReferenceMessenger.Default.Send(new AppMessage<TimerSettings>(timerSettings));
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new AppMessage<bool>(true));
                TimeLeft = string.Empty;
                TimeLeftChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public override TileData[] GetMyHand()
        {
            return GetHandForPlayer(MyName);
        }
        public override void DiscardSelectedTileAndSave(int selectedIndex, Action<Task> onComplete)
        {
            if (!IsFull) { onComplete(Task.CompletedTask); return; }
            if (CurrentPlayerName != MyName) { onComplete(Task.CompletedTask); return; }
            if (!HasDrawnThisTurn) { onComplete(Task.CompletedTask); return; }
            TileData[] hand = GetMyHand();
            Board board = new Board(hand);
            board.EnsureCapacity();
            if (selectedIndex < 0 || selectedIndex >= board.Tiles.Length)
            {
                onComplete(Task.CompletedTask);
                return;
            }
            TileData chosen = board.Tiles[selectedIndex];
            if (chosen == null || chosen.IsEmptyTile)
            {
                onComplete(Task.CompletedTask);
                return;
            }
            DiscardTile = new TileData
            {
                Color = chosen.Color,
                Number = chosen.Number,
                IsJoker = chosen.IsJoker,
                IsEmptyTile = false,
                IsPresent = true
            };
            board.Tiles[selectedIndex] = new TileData
            {
                Color = 0,
                Number = 0,
                IsJoker = false,
                IsEmptyTile = true,
                IsPresent = false
            };
            TileData[] newHand = board.ExportToArray();
            SetHandForPlayer(MyName, newHand);
            Dictionary<string, object> updates = new Dictionary<string, object>();

            updates[GetHandFieldNameForPlayer(MyName)] = newHand;
            updates[nameof(DiscardTile)] = DiscardTile;

            // 1) קודם בודקים ניצחון (לפי היד אחרי הזריקה)
            TrySetGameOverByTurn(newHand, updates);

            // 2) רק אם המשחק לא נגמר — מתקדמים לתור הבא
            bool advanceTurn = true;
            if (IsGameOver)
                advanceTurn = false;

            if (advanceTurn)
            {
                int next = CurrentTurnIndex + 1;
                if (next > Players) next = 1;

                CurrentTurnIndex = next;
                HasDrawnThisTurn = false;
                updates[nameof(HasDrawnThisTurn)] = HasDrawnThisTurn;
                updates[nameof(CurrentTurnIndex)] = CurrentTurnIndex;
            }

            // 3) עדכון לפיירבייס
            fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
        }
        public override void TakeDiscardAndSave(Action<Task> onComplete)
        {
            if (!IsFull) { onComplete(Task.CompletedTask); return; }
            if (CurrentPlayerName != MyName) { onComplete(Task.CompletedTask); return; }
            if (HasDrawnThisTurn) { onComplete(Task.CompletedTask); return; }
            if (DiscardTile == null || !DiscardTile.IsPresent) { onComplete(Task.CompletedTask); return; }
            TileData[] hand = GetMyHand();
            Board board = new Board(hand);
            board.EnsureCapacity();
            TileData tileToAdd = new TileData
            {
                Color = DiscardTile.Color,
                Number = DiscardTile.Number,
                IsJoker = DiscardTile.IsJoker,
                IsEmptyTile = false,
                IsPresent = false
            };
            bool placed = board.PlaceTileInFirstEmpty(tileToAdd);
            if (!placed) { onComplete(Task.CompletedTask); return; }
            TileData[] newHand = board.ExportToArray();
            SetHandForPlayer(MyName, newHand);
            DiscardTile = new TileData { IsPresent = false };
            HasDrawnThisTurn = true;

            Dictionary<string, object> updates = new Dictionary<string, object>();
            updates[GetHandFieldNameForPlayer(MyName)] = newHand;
            updates[nameof(DiscardTile)] = DiscardTile;
            updates[nameof(HasDrawnThisTurn)] = HasDrawnThisTurn;

            fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
        }
        // אריח "משחקי" = קיים ולא ריק
        private bool IsRealTile(TileData t)
        {
            bool result = true;

            if (t == null)
                result = false;
            else if (t.IsEmptyTile)
                result = false;

            return result;
        }


        private bool IsWinningBoard(TileData[] hand)
        {
            bool result = true;

            if (hand == null || hand.Length == 0)
            {
                result = false;
            }
            else
            {
                int i = 0;
                bool foundAtLeastOneSet = false;

                while (i < hand.Length && result)
                {
                    // מדלגים על "ריקים"/לא-קיימים
                    while (i < hand.Length && !IsRealTile(hand[i]))
                        i++;

                    if (i >= hand.Length)
                        break;

                    // קטע רצוף של אריחים קיימים
                    int start = i;
                    while (i < hand.Length && IsRealTile(hand[i]))
                        i++;
                    int end = i - 1;

                    foundAtLeastOneSet = true;

                    bool setOk = IsValidSet(hand, start, end);
                    if (!setOk)
                        result = false;
                }

                if (!foundAtLeastOneSet)
                    result = false;
            }

            return result;
        }

        private bool IsValidSet(TileData[] hand, int start, int end)
        {
            bool result = true;

            int len = end - start + 1;
            if (len < 3)
                result = false;

            // אין "ריקים" בתוך סט (כבר אמור להיות נכון, אבל בטוח)
            if (result)
            {
                for (int i = start; i <= end; i++)
                {
                    if (!IsRealTile(hand[i]))
                        result = false;
                }
            }

            if (result)
            {
                bool isRunOk = IsValidRun(hand, start, end);
                bool isGroupOk = IsValidGroup(hand, start, end);

                if (!isRunOk && !isGroupOk)
                    result = false;
            }

            return result;
        }

        private bool IsValidRun(TileData[] hand, int start, int end)
        {
            bool result = true;

            int len = end - start + 1;

            int jokerCount = 0;
            int nonJokerCount = 0;

            int color = -1;
            int[] nums = new int[len]; // רק מספרים של לא-ג'וקר

            // 1) איסוף: צבע אחד + מספרים + ג'וקרים
            for (int i = start; i <= end; i++)
            {
                TileData t = hand[i];

                if (!IsRealTile(t))
                    result = false;

                if (result)
                {
                    if (t.IsJoker)
                    {
                        jokerCount++;
                    }
                    else
                    {
                        if (color == -1)
                            color = t.Color;
                        else if (t.Color != color)
                            result = false;

                        nums[nonJokerCount] = t.Number;
                        nonJokerCount++;
                    }
                }
            }

            // חייב להיות לפחות אריח אמיתי אחד כדי לדעת צבע/בסיס
            if (result && nonJokerCount == 0)
                result = false;

            // 2) מיון nums (Selection Sort)
            if (result)
            {
                for (int i = 0; i < nonJokerCount - 1; i++)
                {
                    int minIdx = i;
                    for (int j = i + 1; j < nonJokerCount; j++)
                    {
                        if (nums[j] < nums[minIdx])
                            minIdx = j;
                    }
                    int tmp = nums[i];
                    nums[i] = nums[minIdx];
                    nums[minIdx] = tmp;
                }
            }

            // 3) בדיקת תקינות מספרים + חורים
            int gapsNeeded = 0;
            int minNum = 0;
            int maxNum = 0;

            if (result)
            {
                minNum = nums[0];
                maxNum = nums[nonJokerCount - 1];

                // מספרים חייבים להיות 1..13
                if (minNum < 1 || maxNum > 13)
                    result = false;
            }

            if (result)
            {
                for (int i = 1; i < nonJokerCount; i++)
                {
                    int diff = nums[i] - nums[i - 1];
                    if (diff <= 0) // כפילות או לא עולה
                        result = false;
                    else
                        gapsNeeded += (diff - 1);
                }
            }

            // 4) מספיק ג'וקרים להשלים חורים
            if (result)
            {
                if (gapsNeeded > jokerCount)
                    result = false;
            }

            // 5) ג'וקרים עודפים יכולים להרחיב לפני/אחרי, אבל לא לצאת מ-1..13
            if (result)
            {
                int extra = jokerCount - gapsNeeded;

                int roomLeft = minNum - 1;
                int roomRight = 13 - maxNum;

                if (roomLeft + roomRight < extra)
                    result = false;

                int span = (maxNum - minNum + 1);
                if (span > len)
                    result = false;
            }

            return result;
        }

        private bool IsValidGroup(TileData[] hand, int start, int end)
        {
            bool result = true;

            int len = end - start + 1;
            if (len > 4)
                result = false;

            int jokerCount = 0;
            int number = -1;
            int nonJokerCount = 0;

            bool[] usedColors = new bool[4]; // צבעים 0..3

            for (int i = start; i <= end; i++)
            {
                TileData t = hand[i];

                if (!IsRealTile(t))
                    result = false;

                if (result)
                {
                    if (t.IsJoker)
                    {
                        jokerCount++;
                    }
                    else
                    {
                        nonJokerCount++;

                        if (number == -1)
                            number = t.Number;
                        else if (t.Number != number)
                            result = false;

                        int c = t.Color;
                        if (c < 0 || c > 3)
                            result = false;
                        else
                        {
                            if (usedColors[c])
                                result = false;
                            usedColors[c] = true;
                        }
                    }
                }
            }

            // קבוצה לא יכולה להיות רק ג'וקרים
            if (result && nonJokerCount == 0)
                result = false;

            return result;
        }
        private void TrySetGameOverByTurn(TileData[] currentPlayerHand, Dictionary<string, object> updates)
        {
            bool shouldSet = true;

            if (IsGameOver)
                shouldSet = false;

            if (shouldSet)
            {
                bool win = IsWinningBoard(currentPlayerHand);

                if (win)
                {
                    IsGameOver = true;
                    WinnerIndex = CurrentTurnIndex;

                    updates[nameof(IsGameOver)] = true;
                    updates[nameof(WinnerIndex)] = WinnerIndex;
                    GameOver?.Invoke(this,true);
                }
            }
        }
        public override void RefreshUi()
        {
            // 1) טוענים יד שלי
            TileData[] hand = GetMyHand();

            // 2) בונים רשימת UI מוכנה למסך
            UiBoard.Clear();
            for (int i = 0; i < hand.Length; i++)
            {
                Tile t = tileFactory.FromTileData(hand[i]);
                t.Index = i;
                t.IsSelected = (i == SelectedIndex);
                UiBoard.Add(t);
            }

            // 3) בונים DiscardTileSource
            bool hasDiscard = (DiscardTile != null && DiscardTile.IsPresent);

            if (!hasDiscard)
            {
                DiscardTileSource = null;
            }
            else
            {
                if (DiscardTile!.IsJoker)
                {
                    DiscardTileSource = Strings.Joker;
                }
                else
                {
                    Tile t = new Tile((TileModel.Colors)DiscardTile.Color, DiscardTile.Number);
                    DiscardTileSource = t.Source;
                }
            }

            // 4) מודיעים ל-VM “תתעדכן”
            UiChanged?.Invoke(this, EventArgs.Empty);
        }

        public override void TileTapped(int index)
        {
            bool doWork = true;

            // גבולות
            if (index < 0 || index >= UiBoard.Count)
                doWork = false;

            // אם אין בחירה ולחצו על ריק
            if (doWork)
            {
                if (SelectedIndex == -1 && UiBoard[index].IsEmptyTile)
                    doWork = false;
            }

            // לחיצה על אותו אינדקס = ביטול בחירה
            if (doWork)
            {
                if (SelectedIndex == index)
                {
                    SelectedIndex = -1;
                    RefreshUi();
                    doWork = false;
                }
            }

            // אם כבר נבחר משהו - זו החלפה
            if (doWork)
            {
                if (SelectedIndex != -1)
                {
                    int first = SelectedIndex;
                    int second = index;

                    SelectedIndex = -1;

                    // הלוגיקה שלך לשמירה ל-Firestore קיימת ב-HandleTileTap
                    HandleTileTap(first, _ => { });
                    HandleTileTap(second, _ => { });

                    // אחרי שינוי יד - נרענן תצוגה
                    RefreshUi();

                    doWork = false;
                }
            }

            // אחרת זו בחירה ראשונה
            if (doWork)
            {
                SelectedIndex = index;
                RefreshUi();
            }
        }

        public override void DoDiscardSelected()
        {
            int idx = SelectedIndex;
            SelectedIndex = -1;

            DiscardSelectedTileAndSave(idx, _ => { });

            RefreshUi();
        }

        public override void DoTakeDiscard()
        {
            TakeDiscardAndSave(_ => { });

            RefreshUi();
        }

    }
}