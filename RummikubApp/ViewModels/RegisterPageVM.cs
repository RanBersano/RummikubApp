using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using RummikubApp.ModelLogics;
using RummikubApp.Models;
using RummikubApp.Views;
using System.Windows.Input;
namespace RummikubApp.ViewModels
{
    public partial class RegisterPageVM : ObservableObject
    {
        #region Fields
        private readonly User user = new();
        #endregion
        #region Commands
        public ICommand ToggleIsPasswordCommand { get; }
        public ICommand RegisterCommand { get; }
        #endregion
        #region Properties
        public bool IsPassword { get; set; } = true;
        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged();
                (RegisterCommand as Command)?.ChangeCanExecute();
            }
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
        #endregion
        #region Constructor
        public RegisterPageVM()
        {
            RegisterCommand = new Command(Register, CanRegister);
            ToggleIsPasswordCommand = new Command(ToggleIsPassword);
            user.OnAuthComplete += OnAuthComplete;
            user.ShowToastAlert += ShowToastAlert;
        }
        #endregion
        #region Public Methods
        public bool CanRegister()
        {
            return !IsBusy && user.CanRegister();
        }
        #endregion
        #region Private Methods
        private void OnAuthComplete(object? sender, EventArgs e)
        {
            if(Application.Current != null)
            {
                OnPropertyChanged(nameof(IsBusy));
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new HomePage();
                });
            }
        }
        private void ToggleIsPassword()
        {
            IsPassword = !IsPassword;
            OnPropertyChanged(nameof(IsPassword));
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
        private void Register()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                user.Register();
            }
        }
        #endregion
    }
}
