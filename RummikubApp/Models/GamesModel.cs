using Plugin.CloudFirestore;
using RummikubApp.ModelLogics;
using System.Collections.ObjectModel;
namespace RummikubApp.Models
{
    internal class GamesModel
    {
        protected FbData fbd = new();
        protected IListenerRegistration? ilr;

        public bool IsBusy { get; set; }
        public ObservableCollection<Game>? GamesList { get; set; } = [];
        public ObservableCollection<GameSize>? GameSizes { get; set; } = [new GameSize(2), new GameSize(3), new GameSize(4)];
        public EventHandler<bool>? OnGameAdded;
        public EventHandler? OnGamesChanged;
    }
}
