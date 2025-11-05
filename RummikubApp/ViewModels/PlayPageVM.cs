using System.Reactive.Joins;
using System.Windows.Input;

namespace RummikubApp.ViewModels
{
    internal class PlayPageVM
    {
        public ICommand CreateGameCommand { get; }
        public PlayPageVM()
        {
            CreateGameCommand = new Command(CreateGame);
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
