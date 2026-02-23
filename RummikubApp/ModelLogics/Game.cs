using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.CloudFirestore;
using RummikubApp.Models;
namespace RummikubApp.ModelLogics
{
    public class Game : GameModel
    {
        #region Fields
        private readonly Tile tileFactory = new();
        #endregion
        #region Constructors
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
            Board hostBoard = new(first14);
            hostBoard.EnsureCapacity();
            HostHand = hostBoard.ExportToArray();
            DeckData = Deck.ExportToArray();
        }
        public Game()
        {
            RegisterTimer();
        }
        #endregion
        #region Public Methods
        public override string GetOtherPlayerName(int index)
        {
            string result = string.Empty;
            string[] others =[HostName, PlayerName2, PlayerName3, PlayerName4];
            string[] valid = new string[3];
            int count = 0;
            for (int i = 0; i < others.Length; i++)
            {
                string name = others[i];
                if (!string.IsNullOrWhiteSpace(name) && !string.Equals(name, MyName, StringComparison.Ordinal))
                    if (count < valid.Length)
                    {
                        valid[count] = name;
                        count++;
                    }
            }
            if (index >= 0 && index < count)
                result = valid[index];
            return result;
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
            Dictionary<string, object> updates = new()
            {
                [nameof(HasDrawnThisTurn)] = HasDrawnThisTurn,
                [nameof(CurrentTurnIndex)] = CurrentTurnIndex
            };
            fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
        }
        public override void UpdateGuestUser(Action<Task> onComplete)
        {
            bool canContinue = true;
            if (!IsFull)
            {
                if (Deck == null)
                    RebuildDeckFromData();
                Dictionary<string, object> updates = [];
                bool handDealt = false;
                if (string.IsNullOrEmpty(PlayerName2))
                {
                    PlayerName2 = MyName;
                    updates[nameof(PlayerName2)] = PlayerName2;
                    if (Player2Hand == null || Player2Hand.Length == 0)
                    {
                        TileData[] first14 = Deck!.DealTiles(14);
                        Board b = new(first14);
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
                        Board b = new(first14);
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
                        Board b = new(first14);
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
                    canContinue = false;
                }
                if(canContinue)
                {
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
            }
            else
            {
                Task failed = Task.FromException(new InvalidOperationException(Strings.GameFull));
                onComplete(failed);
                return;
            }
            
        }
        public override void HandleTileTap(int tappedIndex, Action<Task> onComplete)
        {
            if (!IsGameOver)
            {
                Board board = GetMyBoard();
                int before = board.SelectedIndex;
                bool changed = board.HandleTap(tappedIndex);
                int after = board.SelectedIndex;
                if (!changed)
                    onComplete(Task.CompletedTask);
                else
                {
                    bool swapped = (before != -1 && after == -1 && before != tappedIndex);
                    if (swapped)
                    {
                        TileData[] newHand = board.ExportToArray();
                        SetHandForPlayer(MyName, newHand);
                        Dictionary<string, object> updates = [];
                        string field = GetHandFieldNameForPlayer(MyName);
                        updates[field] = newHand;
                        fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
                    }
                    else
                        onComplete(Task.CompletedTask);
                }
            }
        }
        public override void TakeTileFromDeckAndSave(Action<Task> onComplete)
        {
            if (HasDrawnThisTurn || IsGameOver)
                onComplete(Task.CompletedTask);
            else
            {
                if (Deck == null)
                    RebuildDeckFromData();
                TileData? drawn = Deck?.DrawTileData();
                if (drawn == null)
                    onComplete(Task.CompletedTask);
                else
                {
                    TileData[] hand = GetHandForPlayer(MyName);
                    Board board = new(hand);
                    bool placed = board.PlaceTileInFirstEmpty(drawn);
                    if (!placed)
                        onComplete(Task.CompletedTask);
                    else
                    {
                        TileData[] newHand = board.ExportToArray();
                        SetHandForPlayer(MyName, newHand);
                        DeckData = Deck!.ExportToArray();
                        HasDrawnThisTurn = true;
                        Dictionary<string, object> updates = new()
                        {
                            [nameof(DeckData)] = DeckData,
                            [nameof(HasDrawnThisTurn)] = HasDrawnThisTurn
                        };
                        string handField = GetHandFieldNameForPlayer(MyName);
                        updates[handField] = newHand;

                        fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
                    }
                }
            }
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
        public override TileData[] GetMyHand()
        {
            return GetHandForPlayer(MyName);
        }
        public override void DoDiscardSelected()
        {
            int idx = SelectedIndex;
            SelectedIndex = -1;

            DiscardSelectedTileAndSave(idx, _ => { });

            RefreshUi();
        }
        public override void DiscardSelectedTileAndSave(int selectedIndex, Action<Task> onComplete)
        {
            if (!IsFull || IsGameOver) { onComplete(Task.CompletedTask); return; }
            if (CurrentPlayerName != MyName) { onComplete(Task.CompletedTask); return; }
            if (!HasDrawnThisTurn) { onComplete(Task.CompletedTask); return; }
            TileData[] hand = GetMyHand();
            Board board = new(hand);
            board.EnsureCapacity();
            if (selectedIndex < 0 || selectedIndex >= board.Tiles.Length)
                onComplete(Task.CompletedTask);
            else
            {
                TileData chosen = board.Tiles[selectedIndex];
                if (chosen == null || chosen.IsEmptyTile)
                    onComplete(Task.CompletedTask);
                else
                {
                    DiscardTile = new TileData
                    {
                        ColorIndex = chosen.ColorIndex,
                        Value = chosen.Value,
                        IsJoker = chosen.IsJoker,
                        IsEmptyTile = false,
                        IsPresent = true
                    };
                    board.Tiles[selectedIndex] = new TileData
                    {
                        ColorIndex = 0,
                        Value = 0,
                        IsJoker = false,
                        IsEmptyTile = true,
                        IsPresent = false
                    };
                    TileData[] newHand = board.ExportToArray();
                    SetHandForPlayer(MyName, newHand);
                    Dictionary<string, object> updates = new()
                    {
                        [GetHandFieldNameForPlayer(MyName)] = newHand,
                        [nameof(DiscardTile)] = DiscardTile
                    };
                    TrySetGameOverByTurn(newHand, updates);
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
                    fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
                }  
            }
        }
        public override bool CanTakeDiscard()
        {
            return IsFull && CurrentPlayerName == MyName && this.DiscardTile != null && this.DiscardTile.IsPresent && !HasDrawnThisTurn;
        }
        public override void DoTakeDiscard()
        {
            TakeDiscardAndSave(_ => { });
            RefreshUi();
        }
        public override void TakeDiscardAndSave(Action<Task> onComplete)
        {
            if (!IsFull || IsGameOver || CurrentPlayerName != MyName || HasDrawnThisTurn || DiscardTile == null || !DiscardTile.IsPresent)
                onComplete(Task.CompletedTask);
            else
            {
                TileData[] hand = GetMyHand();
                Board board = new(hand);
                board.EnsureCapacity();
                TileData tileToAdd = new()
                {
                    ColorIndex = DiscardTile.ColorIndex,
                    Value = DiscardTile.Value,
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
                Dictionary<string, object> updates = new()
                {
                    [GetHandFieldNameForPlayer(MyName)] = newHand,
                    [nameof(DiscardTile)] = DiscardTile,
                    [nameof(HasDrawnThisTurn)] = HasDrawnThisTurn
                };
                fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
                RefreshUi();
            }
        }
        public override void RefreshUi()
        {
            TileData[] hand = GetMyHand();
            UiBoard.Clear();
            for (int i = 0; i < hand.Length; i++)
            {
                Tile t = tileFactory.FromTileData(hand[i]);
                t.Index = i;
                t.IsSelected = (i == SelectedIndex);
                UiBoard.Add(t);
            }
            bool hasDiscard = (DiscardTile != null && DiscardTile.IsPresent);
            if (!hasDiscard)
                DiscardTileSource = null;
            else
            {
                if (DiscardTile!.IsJoker)
                    DiscardTileSource = Strings.Joker;
                else
                {
                    Tile t = new((TileModel.ColorIndexes)DiscardTile.ColorIndex, DiscardTile.Value);
                    DiscardTileSource = t.Source;
                }
            }
            UiChanged?.Invoke(this, EventArgs.Empty);
        }
        public override void TileTapped(int index)
        {
            if (!IsGameOver)
            {
                bool doWork = true;
                if (index < 0 || index >= UiBoard.Count)
                    doWork = false;
                if (doWork)
                    if (SelectedIndex == -1 && UiBoard[index].IsEmptyTile)
                        doWork = false;
                if (doWork)
                    if (SelectedIndex == index)
                    {
                        SelectedIndex = -1;
                        RefreshUi();
                        doWork = false;
                    }
                if (doWork)
                    if (SelectedIndex != -1)
                    {
                        int first = SelectedIndex;
                        int second = index;
                        SelectedIndex = -1;
                        HandleTileTap(first, _ => { });
                        HandleTileTap(second, _ => { });
                        RefreshUi();
                        doWork = false;
                    }
                if (doWork)
                {
                    SelectedIndex = index;
                    RefreshUi();
                }
            }
        }
        #endregion
        #region Private Methods
        protected override TileData[] GetHandForPlayer(string playerName)
        {
            TileData[] result = [];
            if (playerName == HostName)
                result = HostHand;
            if (playerName == PlayerName2)
                result = Player2Hand;
            if (playerName == PlayerName3)
                result = Player3Hand;
            if (playerName == PlayerName4)
                result = Player4Hand;
            return result;
        }
        protected override void SetHandForPlayer(string playerName, TileData[] hand)
        {
            if (playerName == HostName)
                HostHand = hand;
            else if (playerName == PlayerName2)
                Player2Hand = hand;
            else if (playerName == PlayerName3)
                Player3Hand = hand;
            if (playerName == PlayerName4)
                Player4Hand = hand;
        }
        protected override string GetHandFieldNameForPlayer(string playerName)
        {
            string result = string.Empty;
            if (playerName == HostName)
                result = nameof(HostHand);
            if (playerName == PlayerName2)
                result = nameof(Player2Hand);
            if (playerName == PlayerName3)
                result = nameof(Player3Hand);
            if (playerName == PlayerName4)
                result = nameof(Player4Hand);
            return result;
        }
        protected override void OnButtonClicked(object? sender, EventArgs e)
        {
            if (IsFull && CurrentPlayerName == MyName && sender is IndexedButton btn)
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
                OnGameDeleted?.Invoke(this, EventArgs.Empty);
        }
        protected override void OnChange(IDocumentSnapshot? snapshot, Exception? error)
        {
            Game? updated = snapshot?.ToObject<Game>();
            if (updated != null)
            {
                if (updated.IsGameOver && !IsGameOver)
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
        protected override bool IsRealTile(TileData t)
        {
            bool result = true;
            if (t == null)
                result = false;
            else if (t.IsEmptyTile)
                result = false;
            return result;
        }
        protected override bool IsWinningBoard(TileData[] hand)
        {
            bool result = true;
            if (hand == null || hand.Length == 0)
                result = false;
            else
            {
                int i = 0;
                bool foundAtLeastOneSet = false;
                while (i < hand.Length && result)
                {
                    while (i < hand.Length && !IsRealTile(hand[i]))
                        i++;
                    if (i >= hand.Length)
                        break;
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
        protected override bool IsValidSet(TileData[] hand, int start, int end)
        {
            bool result = true;
            int len = end - start + 1;
            if (len < 3)
                result = false;
            if (result)
                for (int i = start; i <= end; i++)
                    if (!IsRealTile(hand[i]))
                        result = false;
            if (result)
            {
                bool isRunOk = IsValidRun(hand, start, end);
                bool isGroupOk = IsValidGroup(hand, start, end);
                if (!isRunOk && !isGroupOk)
                    result = false;
            }
            return result;
        }
        protected override bool IsValidRun(TileData[] hand, int start, int end)
        {
            bool result = true;
            int len = end - start + 1;
            int jokerCount = 0;
            int nonJokerCount = 0;
            int color = -1;
            int[] nums = new int[len];
            for (int i = start; i <= end; i++)
            {
                TileData t = hand[i];
                if (!IsRealTile(t))
                    result = false;
                if (result)
                {
                    if (t.IsJoker)
                        jokerCount++;
                    else
                    {
                        if (color == -1)
                            color = t.ColorIndex;
                        else if (t.ColorIndex != color)
                            result = false;
                        nums[nonJokerCount] = t.Value;
                        nonJokerCount++;
                    }
                }
            }
            if (result && nonJokerCount == 0)
                result = false;
            if (result)
                for (int i = 0; i < nonJokerCount - 1; i++)
                {
                    int minIdx = i;
                    for (int j = i + 1; j < nonJokerCount; j++)
                        if (nums[j] < nums[minIdx])
                            minIdx = j;
                    (nums[minIdx], nums[i]) = (nums[i], nums[minIdx]);
                }
            int gapsNeeded = 0;
            int minNum = 0;
            int maxNum = 0;
            if (result)
            {
                minNum = nums[0];
                maxNum = nums[nonJokerCount - 1];
                if (minNum < 1 || maxNum > 13)
                    result = false;
            }
            if (result)
            {
                for (int i = 1; i < nonJokerCount; i++)
                {
                    int diff = nums[i] - nums[i - 1];
                    if (diff <= 0)
                        result = false;
                    else
                        gapsNeeded += (diff - 1);
                }
            }
            if (result)
            {
                if (gapsNeeded > jokerCount)
                    result = false;
            }
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
        protected override bool IsValidGroup(TileData[] hand, int start, int end)
        {
            bool result = true;
            int len = end - start + 1;
            if (len > 4)
                result = false;
            int jokerCount = 0;
            int value = -1;
            int nonJokerCount = 0;
            bool[] usedColors = new bool[4];
            for (int i = start; i <= end; i++)
            {
                TileData t = hand[i];
                if (!IsRealTile(t))
                    result = false;
                if (result)
                {
                    if (t.IsJoker)
                        jokerCount++;
                    else
                    {
                        nonJokerCount++;
                        if (value == -1)
                            value = t.Value;
                        else if (t.Value != value)
                            result = false;
                        int c = t.ColorIndex;
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
            if (result && nonJokerCount == 0)
                result = false;
            return result;
        }
        protected override void TrySetGameOverByTurn(TileData[] currentPlayerHand, Dictionary<string, object> updates)
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
        #endregion
    }
}