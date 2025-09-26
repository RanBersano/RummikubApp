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
            return !string.IsNullOrWhiteSpace(user.UserName);
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
    }
}
