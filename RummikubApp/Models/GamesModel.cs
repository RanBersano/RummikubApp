using Plugin.CloudFirestore;
using RummikubApp.ModelLogics;
using System.Collections.ObjectModel;
namespace RummikubApp.Models
{
    public abstract class GamesModel
    {
        protected FbData fbd = new();
        protected IListenerRegistration? ilr;
        protected Game? currentGame;
        public Game? CurrentGame { get => currentGame; set => currentGame = value; }
        public bool IsBusy { get; set; }
        public ObservableCollection<Game>? GamesList { get; set; } = [];
        public ObservableCollection<GameSize>? GameSizes { get; set; } = [new GameSize(2), new GameSize(3), new GameSize(4)];
        public GameSize SelectedGameSize { get; set; } = new GameSize();
        public EventHandler<Game>? OnGameAdded;
        public EventHandler? OnGamesChanged;
        public abstract void AddSnapshotListener();
        public abstract void RemoveSnapshotListener();
    }
}
