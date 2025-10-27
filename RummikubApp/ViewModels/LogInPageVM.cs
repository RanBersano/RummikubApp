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
        public ICommand ResetPasswordCommand => new Command(ResetPassword);
        public LogInPageVM()
        {
            LogInCommand = new Command(LogIn, CanLogIn);
            ToggleIsPasswordCommand = new Command(ToggleIsPassword);
            user.OnAuthComplete += OnAuthComplete;
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
        public async void ResetPassword()
        {
            string resetEmail = await Shell.Current.DisplayPromptAsync(
                Strings.ResetPWPrompt,
                Strings.ResetEmailPrompt,
                Strings.Ok,
                Strings.Cancel,
                maxLength: 50,
                keyboard: Microsoft.Maui.Keyboard.Email
            );
            ResetEmail = resetEmail;
            user.ResetPassword();
        }
        public string ResetEmail
        {
            get => user.ResetEmail;
            set
            {
                user.ResetEmail = value;
            }
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
