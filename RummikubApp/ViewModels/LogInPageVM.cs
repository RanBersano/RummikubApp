using RummikubApp.ModelLogics;
using System.Windows.Input;
using RummikubApp.Models;
using RummikubApp.Views;

namespace RummikubApp.ViewModels
{
    internal class LogInPageVM : ObservableObject
    {
        public bool IsPassword { get; set; } = true;
        private readonly User user = new();
        public ICommand ToggleIsPasswordCommand { get; }
        public ICommand LogInCommand {  get; }
        public ICommand ForgotPasswordCommand { get; }
        public bool IsBusy { get; set; } = false;
        public LogInPageVM()
        {
            LogInCommand = new Command(LogIn, CanLogIn);
            ToggleIsPasswordCommand = new Command(ToggleIsPassword);
            user.OnAuthComplete += OnAuthComplete;
            ForgotPasswordCommand = new Command(async () => await ForgotPassword());
        }
        private async Task ForgotPassword()
        {
            await user.ResetPassword();
        }
        public bool CanLogIn()
        {
            return user.CanLogIn();
        }
        private void OnAuthComplete(object? sender, EventArgs e)
        {
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
            user.LogIn();
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
