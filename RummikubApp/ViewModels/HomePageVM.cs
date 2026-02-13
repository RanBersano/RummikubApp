using CommunityToolkit.Maui.Views;
using RummikubApp.ModelLogics;
using RummikubApp.Models;
using RummikubApp.Views;
using System.Windows.Input;

namespace RummikubApp.ViewModels
{
    public class HomePageVM
    {
        private readonly User user = new();
        public ICommand PlayCommand { get; }
        public ICommand InstructionsPopupCommand { get; private set; }
        public HomePageVM()
        {
            PlayCommand = new Command(Play);
            InstructionsPopupCommand = new Command(InstructionsPopup);
        }
        private void Play (object? sender)
        {
            if (Application.Current != null)
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new AppShell();
                });
        }
        private void InstructionsPopup(object obj)
        {
            Application.Current!.MainPage!.DisplayAlert(Strings.Instructions, Strings.RummyInstructions, Strings.Ok);
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
