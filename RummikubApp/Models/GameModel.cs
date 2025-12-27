using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using RummikubApp.ModelLogics;

namespace RummikubApp.Models
{
    public abstract class GameModel
    {
        protected FbData fbd = new();
        protected IListenerRegistration? ilr;

        [Ignored] public EventHandler? OnGameChanged;
        [Ignored] public EventHandler? OnGameDeleted;
        [Ignored] public EventHandler? TimeLeftChanged;
        [Ignored] public string TimeLeft { get; protected set; } = string.Empty;
        [Ignored] public string Id { get; set; } = string.Empty;

        public DateTime Created { get; set; }
        public int Players { get; set; }
        public bool IsFull { get; set; }
        public int CurrentNumOfPlayers { get; set; } = 1;

        public string HostName { get; set; } = string.Empty;
        public string PlayerName2 { get; set; } = string.Empty;
        public string PlayerName3 { get; set; } = string.Empty;
        public string PlayerName4 { get; set; } = string.Empty;

        public int CurrentTurnIndex { get; set; } = 1;
        public bool HasDrawnThisTurn { get; set; } = false;

        // ✅ Firestore state - Arrays בלבד
        public TileData[] DeckData { get; set; } = new TileData[0];
        public TileData[] HostHand { get; set; } = new TileData[0];
        public TileData[] Player2Hand { get; set; } = new TileData[0];
        public TileData[] Player3Hand { get; set; } = new TileData[0];
        public TileData[] Player4Hand { get; set; } = new TileData[0];
        public TileData DiscardTile { get; set; } = new TileData { IsPresent = false };

        [Ignored] public ModelLogics.Deck? Deck { get; set; }

        [Ignored] public string MyName { get; set; } = new User().UserName;
        [Ignored] public bool IsHostUser { get; set; }

        [Ignored]
        public string CurrentPlayerName
        {
            get
            {
                return CurrentTurnIndex switch
                {
                    1 => HostName,
                    2 => PlayerName2,
                    3 => PlayerName3,
                    4 => PlayerName4,
                    _ => HostName
                };
            }
        }
        protected TimerSettings timerSettings = new(Keys.TimerTotalTime, Keys.TimerInterval);
        public abstract void SetDocument(Action<Task> onComplete);
        public abstract void AddSnapshotListener();
        public abstract void RemoveSnapshotListener();
        public abstract void DeleteDocument(Action<Task> onComplete);

        public abstract void InitGrid(Grid deck);
        protected abstract void OnButtonClicked(object? sender, EventArgs e);

        public abstract void MoveToNextTurn(Action<Task> onComplete);

        public abstract string GetOtherPlayerName(int index);
        public abstract void UpdateGuestUser(Action<Task> onComplete);

        // ⬇️ Board/Deck actions
        public abstract void HandleTileTap(int tappedIndex, Action<Task> onComplete);
        public abstract void TakeTileFromDeckAndSave(Action<Task> onComplete);

        protected abstract void OnComplete(Task task);
        protected abstract void OnChange(IDocumentSnapshot? snapshot, Exception? error);
        public abstract void DiscardSelectedTileAndSave(int selectedIndex, Action<Task> onComplete);
        public abstract void TakeDiscardAndSave(Action<Task> onComplete);

    }
}