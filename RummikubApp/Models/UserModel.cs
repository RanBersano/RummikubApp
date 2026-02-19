using RummikubApp.ModelLogics;
namespace RummikubApp.Models
{
    public abstract class UserModel
    {
        protected FbData fbd = new();
        public EventHandler? OnAuthComplete;
        public EventHandler<string>? ShowToastAlert;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ResetEmail { get; set; } = string.Empty;
        public bool IsRegistered => (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Email));
        public bool IsBusy { get; protected set; } = false;
        public abstract void Login(bool IsChecked);
        public abstract void Register();
        protected abstract void OnComplete(Task task);
        protected abstract void ShowAlert(string msg);
        protected abstract void SaveToPreferences();
        public abstract bool CanLogIn();
        public abstract bool CanRegister();
        public abstract void ResetEmailPassword();
        protected abstract void OnResetComplete(Task task);
        public abstract string GetFirebaseErrorMessage(string msg);
        public string EmailForReset = string.Empty;

    }
}
