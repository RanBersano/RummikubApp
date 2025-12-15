using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Plugin.CloudFirestore;
using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    public class Game : GameModel
    {
        public Game(GameSize selectedGameSize)
        {
            HostName = new User().UserName;
            Players = selectedGameSize.Size;
            Created = DateTime.Now;
            CurrentNumOfPlayers = 1;

            Deck = new Deck();                 // בונה קופה מלאה ומערבב
            DeckData = new List<TileData>(Deck.Tiles);

            HostHand = Deck.DealTiles(14);     // מחלק יד למארח
            DeckData = new List<TileData>(Deck.Tiles);
        }
        public Game()
        {
        }
        private void RebuildDeckFromData()
        {
            Deck = new Deck(DeckData);
        }
        public override string GetOtherPlayerName(int index)
        {
            string[] others = new string[]
            {
                HostName,
                PlayerName2,
                PlayerName3,
                PlayerName4
            };

            List<string> validNames = new List<string>();
            for (int i = 0; i < others.Length; i++)
            {
                string name = others[i];
                if (!string.IsNullOrWhiteSpace(name) &&
                    !string.Equals(name, MyName, StringComparison.Ordinal))
                {
                    validNames.Add(name);
                } 
            }

            if (index < validNames.Count)
                return validNames[index];
            else
                return string.Empty;
        }
        private int selectedIndex = -1;
        public void HandleTileTap(int tappedIndex, Action<Task> onComplete)
        {
            List<TileData> hand = GetHandDataForPlayer(MyName);
            if (tappedIndex < 0 || tappedIndex >= hand.Count)
            {
                return;
            }
            // בחירה ראשונה – לא לבחור מקום ריק
            if (selectedIndex == -1)
            {
                if (hand[tappedIndex].IsEmptySlot)
                {
                    onComplete(Task.CompletedTask);
                    return;
                }

                selectedIndex = tappedIndex;
                hand[tappedIndex].IsSelected = true; // צריך שדה כזה ב-TileData (ראה למטה)
                UpdateHandForPlayer(MyName, hand, onComplete);
                return;
            }

            // לחיצה חוזרת על אותו אריח – ביטול בחירה
            if (selectedIndex == tappedIndex)
            {
                hand[tappedIndex].IsSelected = false;
                selectedIndex = -1;
                UpdateHandForPlayer(MyName, hand, onComplete);
                return;
            }

            // החלפה (גם אם אחד ריק)
            TileData first = hand[selectedIndex];
            TileData second = hand[tappedIndex];

            first.IsSelected = false;

            hand[selectedIndex] = second;
            hand[tappedIndex] = first;

            selectedIndex = -1;

            UpdateHandForPlayer(MyName, hand, onComplete);
        }

        public List<TileData> GetHandDataForPlayer(string playerName)
        {
            if (playerName == HostName)
            {
                return HostHand ?? new List<TileData>();
            }

            if (playerName == PlayerName2)
            {
                return Player2Hand ?? new List<TileData>();
            }

            if (playerName == PlayerName3)
            {
                return Player3Hand ?? new List<TileData>();
            }

            if (playerName == PlayerName4)
            {
                return Player4Hand ?? new List<TileData>();
            }

            return new List<TileData>();
        }
        public override void MoveToNextTurn(Action<Task> onComplete)
        {
            int next = CurrentTurnIndex + 1;
            if (next > Players)
                next = 1;

            CurrentTurnIndex = next;

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict[nameof(CurrentTurnIndex)] = CurrentTurnIndex;

            fbd.UpdateFields(Keys.GamesCollection, Id, dict, onComplete);
        }
        public override void SetDocument(Action<Task> OnComplete)
        {
            Id = fbd.SetDocument(this, Keys.GamesCollection, Id, OnComplete);
        }
        public override void UpdateGuestUser(Action<Task> onComplete)
        {
            if (IsFull)
            {
                Task failedTask = Task.FromException(
                    new InvalidOperationException(Strings.GameFull));
                onComplete(failedTask);
                return;
            }

            Dictionary<string, object> updates = new Dictionary<string, object>();

            bool handDealt = false;

            if (Deck == null)
            {
                // אם Deck עדיין לא נבנה בצד הזה (למשל אצל אורח) – נבנה אותו מתוך DeckData
                RebuildDeckFromData();
            }

            if (string.IsNullOrEmpty(PlayerName2))
            {
                PlayerName2 = MyName;
                updates[nameof(PlayerName2)] = PlayerName2;

                if (Player2Hand == null || Player2Hand.Count == 0)
                {
                    Player2Hand = Deck!.DealTiles(14);
                    updates[nameof(Player2Hand)] = Player2Hand;
                    handDealt = true;
                }
            }
            else if (Players >= 3 && string.IsNullOrEmpty(PlayerName3))
            {
                PlayerName3 = MyName;
                updates[nameof(PlayerName3)] = PlayerName3;

                if (Player3Hand == null || Player3Hand.Count == 0)
                {
                    Player3Hand = Deck!.DealTiles(14);
                    updates[nameof(Player3Hand)] = Player3Hand;
                    handDealt = true;
                }
            }
            else if (Players == 4 && string.IsNullOrEmpty(PlayerName4))
            {
                PlayerName4 = MyName;
                updates[nameof(PlayerName4)] = PlayerName4;

                if (Player4Hand == null || Player4Hand.Count == 0)
                {
                    Player4Hand = Deck!.DealTiles(14);
                    updates[nameof(Player4Hand)] = Player4Hand;
                    handDealt = true;
                }
            }
            else
            {
                Task failedTask = Task.FromException(
                    new InvalidOperationException(Strings.GameFull));
                onComplete(failedTask);
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
                DeckData = new List<TileData>(Deck!.Tiles);
                updates[nameof(DeckData)] = DeckData;
            }

            fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
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
        protected override void OnComplete(Task task)
        {
            if(task.IsCompletedSuccessfully)
                OnGameDeleted?.Invoke(this, EventArgs.Empty);
        }
        protected override void OnChange(IDocumentSnapshot? snapshot, Exception? error)
        {
            Game? updatedGame = snapshot?.ToObject<Game>();
            if (updatedGame != null)
            {
                Players = updatedGame.Players;
                IsFull = updatedGame.IsFull;
                CurrentNumOfPlayers = updatedGame.CurrentNumOfPlayers;
                HostName = updatedGame.HostName;
                PlayerName2 = updatedGame.PlayerName2;
                PlayerName3 = updatedGame.PlayerName3;
                PlayerName4 = updatedGame.PlayerName4;
                CurrentTurnIndex = updatedGame.CurrentTurnIndex;
                DeckData = updatedGame.DeckData ?? new List<TileData>();
                HostHand = updatedGame.HostHand ?? new List<TileData>();
                Player2Hand = updatedGame.Player2Hand ?? new List<TileData>();
                Player3Hand = updatedGame.Player3Hand ?? new List<TileData>();
                Player4Hand = updatedGame.Player4Hand ?? new List<TileData>();
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
        }
        public override void DeleteDocument(Action<Task> onComplete)
        {
            fbd.DeleteDocument(Keys.GamesCollection, Id, onComplete);
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

        protected override void OnButtonClicked(object? sender, EventArgs e)
        {
            if (!IsFull)
            {
                return;
            }
            if (CurrentPlayerName != MyName)
            {
                return;
            }
            IndexedButton? btn = sender as IndexedButton;
            if (btn == null)
            {
                return;
            }
            TakeTileFromDeckAndSave(OnTakeTileComplete);
        }
        private void OnTakeTileComplete(Task task)
        {
            if (!task.IsCompletedSuccessfully)
            {
                return;
            }
            MoveToNextTurn(OnCompleteTurn);
        }
        public string GetHandFieldNameForPlayer(string playerName)
        {
            if (playerName == HostName)
            {
                return nameof(HostHand);
            }

            if (playerName == PlayerName2)
            {
                return nameof(Player2Hand);
            }

            if (playerName == PlayerName3)
            {
                return nameof(Player3Hand);
            }

            if (playerName == PlayerName4)
            {
                return nameof(Player4Hand);
            }

            return string.Empty;
        }
        private List<TileData> GetHandListByName(string playerName)
        {
            if (playerName == HostName)
            {
                return HostHand;
            }

            if (playerName == PlayerName2)
            {
                return Player2Hand;
            }

            if (playerName == PlayerName3)
            {
                return Player3Hand;
            }

            if (playerName == PlayerName4)
            {
                return Player4Hand;
            }

            return new List<TileData>();
        }

        private void SetHandListByName(string playerName, List<TileData> hand)
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
            {
                Player4Hand = hand;
                return;
            }
        }

        public void UpdateHandForPlayer(string playerName, List<TileData> newHand, Action<Task> onComplete)
        {
            string fieldName = GetHandFieldNameForPlayer(playerName);
            //if (string.IsNullOrEmpty(fieldName))
            //{
            //    onComplete(Task.FromException(new InvalidOperationException("Player not found")));
            //    return;
            //}

            Dictionary<string, object> updates = new Dictionary<string, object>();
            updates[fieldName] = newHand;

            fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
        }
        private void OnCompleteTurn(Task task)
        {
            // במידה ותרצה Toast / טיפול בשגיאה
        }

        public void TakeTileFromDeckAndSave(Action<Task> onComplete)
        {
            //if (!IsFull)
            //{
            //    onComplete(Task.FromException(new InvalidOperationException("Game is not full")));
            //    return;
            //}

            //if (CurrentPlayerName != MyName)
            //{
            //    onComplete(Task.FromException(new InvalidOperationException("Not your turn")));
            //    return;
            //}

            //if (Deck == null)
            //{
            //    RebuildDeckFromData();
            //}

            //if (Deck == null)
            //{
            //    onComplete(Task.FromException(new InvalidOperationException("Deck not availabe")));
            //    return;
            //}

            TileData? drawn = Deck?.DrawTileData();
            //if (drawn == null)
            //{
            //    onComplete(Task.FromException(new InvalidOperationException("Deck is empty")));
            //    return;
            //}

            List<TileData> hand = GetHandListByName(MyName);
            //if (hand == null)
            //{
            //    hand = new List<TileData>();
            //}

            int emptyIndex = -1;
            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i] != null && hand[i].IsEmptySlot)
                {
                    emptyIndex = i;
                    break;
                }
            }

            //if (emptyIndex == -1)
            //{
            //    onComplete(Task.FromException(new InvalidOperationException("No empty slot in hand")));
            //    return;
            //}

            drawn!.IsEmptySlot = false;
            hand[emptyIndex] = drawn;

            SetHandListByName(MyName, hand);

            DeckData = new List<TileData>(Deck!.Tiles);

            Dictionary<string, object> updates = new Dictionary<string, object>();
            updates[nameof(DeckData)] = DeckData;

            string handFieldName = GetHandFieldNameForPlayer(MyName);
            updates[handFieldName] = hand;

            fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
        }

        protected override void TakeTileFromDeck()
        {
        }
    }
}
