using RummikubApp.ModelLogics;
using RummikubApp.Models;
using RummikubApp.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;
namespace RummikubApp.ViewModels
{
    public partial class MainPageVM : ObservableObject
    {
        #region Fields
        private readonly User user = new();
        private readonly Games games = new();
        #endregion
        #region Commands
        public ICommand AddGameCommand => new Command(AddGame);
        #endregion
        #region Properties
        public ObservableCollection<GameSize>? GameSizes { get => games.GameSizes; set => games.GameSizes = value; }
        public ObservableCollection<Game>? GamesList => games.GamesList;
        public GameSize SelectedGameSize { get => games.SelectedGameSize; set => games.SelectedGameSize = value; }
        public bool IsBusy => games.IsBusy;
        public Game? SelectedItem
        {
            get => games.CurrentGame;
            set
            {
                if (value != null)
                {
                    games.CurrentGame = value;
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Shell.Current.Navigation.PushAsync(new GamePage(value), true);
                    });
                }
            }
        }
        #endregion
        #region Constructor
        public MainPageVM()
        {
            games.OnGameAdded += OnGameAdded;
            games.OnGamesChanged += OnGamesChanged;
        }
        #endregion
        #region Public Methods
        public void AddSnapshotListener()
        {
            games.AddSnapshotListener();
        }
        public void RemoveSnapshotListener()
        {
            games.RemoveSnapshotListener();
        }
        public string UserName
        {
            get => user.UserName;
            set => user.UserName = value;
        }
        #endregion
        #region Private Methods
        private void AddGame()
        {
            games.AddGame();
            OnPropertyChanged(nameof(IsBusy));
        }
        private void OnGamesChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(GamesList));
        }
        private void OnGameAdded(object? sender, Game game)
        {
            OnPropertyChanged(nameof(IsBusy));
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Shell.Current.Navigation.PushAsync(new GamePage(game), true);
            });
        }
        #endregion
    }
}
