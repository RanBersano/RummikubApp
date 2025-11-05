using RummikubApp.ModelLogics;
using RummikubApp.Models;
using RummikubApp.Views;
using System.Windows.Input;

namespace RummikubApp.ViewModels
{
    internal class HomePageVM
    {
        private readonly User user = new();
        public ICommand PlayCommand { get; }
        public HomePageVM()
        {
            PlayCommand = new Command(Play);
        }

        private void Play (object? sender)
        {
            if (Application.Current != null)
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new PlayPage();
                });
            }
        }

        public string UserName
        {
            get => user.UserName;
            set
            {
                user.UserName = value;
            }
        }
    }
}
