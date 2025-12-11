using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using RummikubApp.ModelLogics;
namespace RummikubApp.Models
{
    public abstract class GameModel
    {
        protected FbData fbd = new();
        protected IListenerRegistration? ilr;
        [Ignored]
        public EventHandler? OnGameChanged;
        [Ignored]
        public EventHandler? OnGameDeleted;
        [Ignored]
        public string Id { get; set; } = string.Empty; 
        public DateTime Created { get; set; }
        public int Players { get; set; }
        public bool IsFull { get; set; }
        public bool IsHostTurn { get; set; } = false;
        public int CurrentNumOfPlayers { get; set; } = 1;
        public string HostName { get; set; } = string.Empty;
        public string PlayerName2 { get; set; } = string.Empty;
        public string PlayerName3 { get; set; } = string.Empty;
        public string PlayerName4 { get; set; } = string.Empty;
        public int CurrentTurnIndex { get; set; } = 1;
        public List<TileData> DeckData { get; set; } = new List<TileData>();
        public List<TileData> HostHand { get; set; } = new List<TileData>();
        public List<TileData> Player2Hand { get; set; } = new List<TileData>();
        public List<TileData> Player3Hand { get; set; } = new List<TileData>();
        public List<TileData> Player4Hand { get; set; } = new List<TileData>();
        [Ignored]
        public Deck? Deck { get; set; }
        [Ignored]
        public string MyName { get; set; } = new User().UserName; 
        [Ignored]
        public bool IsHostUser { get; set; }
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
        public abstract void SetDocument(Action<System.Threading.Tasks.Task> OnComplete);
        public abstract void AddSnapshotListener();
        public abstract void RemoveSnapshotListener();
        public abstract void DeleteDocument(Action<Task> onComplete);
        public abstract void InitGrid(Grid deck);
        protected abstract void OnButtonClicked(object? sender, EventArgs e);
        protected abstract void TakeTileFromDeck();
        public abstract void MoveToNextTurn(Action<Task> onComplete);
        public abstract string GetOtherPlayerName(int index);
        public abstract void UpdateGuestUser(Action<Task> onComplete);
        protected abstract void OnComplete(Task task);
        protected abstract void OnChange(IDocumentSnapshot? snapshot, Exception? error);
    }
}
