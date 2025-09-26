using RummikubApp.ModelLogics;
using System.Windows.Input;

namespace RummikubApp.ViewModels
{
    internal class RegisterPageVM
    {
        private readonly User user = new();
        public ICommand RegisterCommand { get; }
        public RegisterPageVM()
        {
            RegisterCommand = new Command(Register, CanRegister);
        }
        public bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Email);
        }

        private void Register()
        {
            user.Register();
        }

        public string UserName
        {
            get => user.UserName;
            set
            {
                user.UserName = value;
                (RegisterCommand as Command)?.ChangeCanExecute();
            }
        }
        public string Password
        {
            get => user.Password;
            set
            {
                user.Password = value;
                (RegisterCommand as Command)?.ChangeCanExecute();
            }
        }
        public string Email
        {
            get => user.Email;
            set
            {
                user.Email = value;
                (RegisterCommand as Command)?.ChangeCanExecute();
            }
        }
    }
}
