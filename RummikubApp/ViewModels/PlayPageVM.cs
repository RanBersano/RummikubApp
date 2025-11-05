using RummikubApp.ModelLogics;
using RummikubApp.Models;
using System.Collections.ObjectModel;
using System.Reactive.Joins;
using System.Windows.Input;

namespace RummikubApp.ViewModels
{
    internal class PlayPageVM : ObservableObject
    {
        public ICommand CreateGameCommand { get; }
        private readonly Games games = new();
        public bool IsBusy => games.IsBusy;
        public ObservableCollection<GameSize>? GameSizes { get => games.GameSizes; set => games.GameSizes = value; }
        public GameSize SelectedGameSize { get; set; } = new GameSize();
        public ICommand AddGameCommand => new Command(AddGame);

        private void AddGame()
        {
            games.AddGame(SelectedGameSize);
            OnPropertyChanged(nameof(IsBusy));
        }
        public ObservableCollection<Game>? GamesList => games.GamesList;
        public PlayPageVM()
        {
            CreateGameCommand = new Command(CreateGame);
            games.OnGameAdded += OnGameAdded;
            games.OnGamesChanged += OnGamesChanged;
        }

        private void OnGamesChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(GamesList));
        }

        private void OnGameAdded(object? sender, bool e)
        {
            OnPropertyChanged(nameof(IsBusy));
        }
        internal void AddSnapshotListener()
        {
            games.AddSnapshotListener();
        }

        internal void RemoveSnapshotListener()
        {
            games.RemoveSnapshotListener();
        }
        private void CreateGame(object? sender)
        {
            if (Application.Current != null)
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new AppShell();
                });
            }
        }
    }
}