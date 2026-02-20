using RummikubApp.Views;
using System.Windows.Input;
namespace RummikubApp.ViewModels
{
    public class GameOverPopUpVM
    {
        #region Fields
        private readonly GameOverPopUp _gameOverPopUp;
        #endregion
        #region Commands
        public ICommand HomeCommand { get; private set; }
        #endregion
        #region Properties
        public string ResultText { get; }
        public string ResultMessage { get; }
        #endregion
        #region Constructor
        public GameOverPopUpVM(GameOverPopUp gameOverPopUp, string title, string message)
        {
            _gameOverPopUp = gameOverPopUp;
            ResultText = title;
            ResultMessage = message;
            HomeCommand = new Command(TransferHome);
        }
        #endregion
        #region Private Methods
        private void TransferHome(object obj)
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (Application.Current != null)
                    Application.Current.MainPage = new HomePage();
            });
            _gameOverPopUp.Close();
        }
        #endregion
    }
}
