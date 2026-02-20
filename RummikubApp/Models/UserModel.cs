using RummikubApp.ModelLogics;
namespace RummikubApp.Models
{
    public abstract class UserModel
    {
        #region Fields
        protected FbData fbd = new();
        #endregion
        #region Events
        public EventHandler? OnAuthComplete;
        public EventHandler<string>? ShowToastAlert;
        #endregion
        #region Properties
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ResetEmail { get; set; } = string.Empty;
        public string EmailForReset { get; set; } = string.Empty;
        public bool IsRegistered => (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Email));
        public bool IsBusy { get; protected set; } = false;
        #endregion
        #region Public Methods
        public abstract void Login(bool IsChecked);
        public abstract void Register();
        public abstract bool CanLogIn();
        public abstract bool CanRegister();
        public abstract void ResetEmailPassword();
        public abstract string GetFirebaseErrorMessage(string msg);
        #endregion
        #region Private Methods
        protected abstract void OnComplete(Task task);
        protected abstract void ShowAlert(string msg);
        protected abstract void SaveToPreferences();
        protected abstract void OnResetComplete(Task task);
        #endregion
    }
}
