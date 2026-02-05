using RummikubApp.ModelLogics;
using System.Windows.Input;
using RummikubApp.Models;
using RummikubApp.Views;

namespace RummikubApp.ViewModels
{
    public class LogInPageVM : ObservableObject
    {
        public bool IsPassword { get; set; } = true;
        private readonly User user = new();
        public bool IsBusy { get; set; } = false;
        public ICommand ResetEmail => new Command(ResetPass);
        public ICommand ToggleIsPasswordCommand { get; }
        public ICommand LogInCommand {  get; }
        private bool IsCheckedValue;
        public LogInPageVM()
        {
            LogInCommand = new Command(LogIn, CanLogIn);
            ToggleIsPasswordCommand = new Command(ToggleIsPassword);
            user.OnAuthComplete += OnAuthComplete;
        }
        public bool IsChecked
        {
            get => IsCheckedValue;
            set => IsCheckedValue = value;
        }
        public string EmailForReset
        {
            get => user.EmailForReset;
            set
            {
                user.EmailForReset = value;
            }
        }
        private void ResetPass()
        {
            user.EmailForReset = EmailForReset;
            user.ResetEmailPassword();
        }
        public bool CanLogIn()
        {
            return user.CanLogIn();
        }
        private void OnAuthComplete(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsBusy));
            if (Application.Current != null)
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new HomePage();
                });
            }
        }
        private void LogIn()
        {
            user.Login(IsCheckedValue);
        }
        private void ToggleIsPassword()
        {
            IsPassword = !IsPassword;
            OnPropertyChanged(nameof(IsPassword));
        }
        public string UserName 
        { 
            get => user.UserName;
            set
            {
                user.UserName = value;
                (LogInCommand as Command)?.ChangeCanExecute();
            }
        }
        public string Password
        {
            get => user.Password;
            set
            {
                user.Password = value;
                (LogInCommand as Command)?.ChangeCanExecute();
            }
        }
        public string Email
        {
            get => user.Email;
            set
            {
                user.Email = value;
                (LogInCommand as Command)?.ChangeCanExecute();
            }
        }
    }
}
