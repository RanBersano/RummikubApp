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
        protected abstract GameStatusModel Status { get; }
        [Ignored]
        public string StatusMessage => Status.StatusMessage;
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
        [Ignored]
        public string MyName { get; set; } = new User().UserName; 
        [Ignored]
        public bool IsHostUser { get; set; }
        public abstract void SetDocument(Action<System.Threading.Tasks.Task> OnComplete);
        public abstract void AddSnapshotListener();
        public abstract void RemoveSnapshotListener();
        public abstract void DeleteDocument(Action<Task> onComplete);
        public abstract void InitGrid(Grid deck);
    }
}
