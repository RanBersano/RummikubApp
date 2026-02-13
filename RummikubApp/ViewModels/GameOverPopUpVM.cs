using RummikubApp.Views;
using System.Windows.Input;
namespace RummikubApp.ViewModels
{
    public class GameOverPopUpVM
    {
        private readonly GameOverPopUp _gameOverPopUp;
        public ICommand HomeCommand { get; private set; }
        public string ResultText { get; }
        public string ResultMessage { get; }
        public GameOverPopUpVM(GameOverPopUp gameOverPopUp, string title, string message)
        {
            _gameOverPopUp = gameOverPopUp;
            ResultText = title;
            ResultMessage = message;
            HomeCommand = new Command(TransferHome);
        }
        private void TransferHome(object obj)
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (Application.Current != null)
                    Application.Current.MainPage = new HomePage();
            });
            _gameOverPopUp.Close();
        }
    }
}
