using Plugin.CloudFirestore;
using RummikubApp.ModelLogics;
using System.Collections.ObjectModel;
namespace RummikubApp.Models
{
    public abstract class GamesModel
    {
        #region Fields
        protected FbData fbd = new();
        protected IListenerRegistration? ilr;
        protected Game? currentGame;
        #endregion
        #region Events
        public EventHandler<Game>? OnGameAdded;
        public EventHandler? OnGamesChanged;
        #endregion
        #region Properties
        public Game? CurrentGame { get => currentGame; set => currentGame = value; }
        public ObservableCollection<Game>? GamesList { get; set; } = [];
        public ObservableCollection<GameSize>? GameSizes { get; set; } = [new GameSize(2), new GameSize(3), new GameSize(4)];
        public GameSize SelectedGameSize { get; set; } = new GameSize();
        public bool IsBusy { get; set; }
        #endregion
        #region Public Methods
        public abstract void AddGame();
        public abstract void AddSnapshotListener();
        public abstract void RemoveSnapshotListener();
        #endregion
        #region Private Methods
        protected abstract void OnGameDeleted(object? sender, EventArgs e);
        protected abstract void OnComplete(Task task);
        protected abstract void OnChange(IQuerySnapshot snapshot, Exception error);
        protected abstract void OnComplete(IQuerySnapshot qs);
        #endregion
    }
}
