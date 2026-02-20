using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using RummikubApp.ModelLogics;
using RummikubApp.Models;
using RummikubApp.Views;
using System.Windows.Input;

namespace RummikubApp.ViewModels
{
    public partial class LogInPageVM : ObservableObject
    {
        #region Fields
        private readonly User user = new();
        #endregion
        #region Commands
        public ICommand ResetEmail => new Command(ResetPass);
        public ICommand ToggleIsPasswordCommand { get; }
        public ICommand LogInCommand {  get; }
        #endregion
        #region Properties
        private bool IsCheckedValue { get; set; }
        public bool IsPassword { get; set; } = true;
        public bool IsChecked
        {
            get => IsCheckedValue;
            set => IsCheckedValue = value;
        }
        public string EmailForReset
        {
            get => user.EmailForReset;
            set => user.EmailForReset = value;
        }
        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged();
                (LogInCommand as Command)?.ChangeCanExecute();
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
        #endregion
        #region Constructor
        public LogInPageVM()
        {
            LogInCommand = new Command(LogIn, CanLogIn);
            ToggleIsPasswordCommand = new Command(ToggleIsPassword);
            user.OnAuthComplete += OnAuthComplete;
            user.ShowToastAlert += ShowToastAlert;
        }
        #endregion
        #region Private Methods
        public bool CanLogIn()
        {
            return !IsBusy && user.CanLogIn();
        }
        private void ResetPass()
        {
            user.EmailForReset = EmailForReset;
            user.ResetEmailPassword();
        }
        private void ShowToastAlert(object? sender, string msg)
        {
            isBusy = false;
            OnPropertyChanged(nameof(isBusy));
            OnPropertyChanged(nameof(isBusy));
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Toast.Make(msg, ToastDuration.Long).Show();
            });
        }
        private void OnAuthComplete(object? sender, EventArgs e)
        {
            if (Application.Current != null)
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new HomePage();
                });
        }
        private void LogIn()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                user.Login(IsCheckedValue);
            }
        }
        private void ToggleIsPassword()
        {
            IsPassword = !IsPassword;
            OnPropertyChanged(nameof(IsPassword));
        }
        #endregion
    }
}
