using RummikubApp.ModelLogics;
using System.Windows.Input;

namespace RummikubApp.ViewModels
{
    internal class LogInPageVM
    {
        private readonly User user = new();
        public ICommand LogInCommand {  get; }
        public LogInPageVM()
        {
            LogInCommand = new Command(LogIn, CanLogIn);
        }
        public bool CanLogIn()
        {
            return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Email);
        }

        private void LogIn()
        {
            user.LogIn();
        }

        public string UserName 
        { 
            get => user.UserName ;
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
