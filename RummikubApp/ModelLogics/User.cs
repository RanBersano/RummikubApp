using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using RummikubApp.Models;
namespace RummikubApp.ModelLogics
{
    public class User : UserModel
    {
        #region Constructor
        public User()
        {
            UserName = Preferences.Get(Keys.UserNameKey, string.Empty);
            Password = Preferences.Get(Keys.PasswordKey, string.Empty);
            Email = Preferences.Get(Keys.EmailKey, string.Empty);
        }
        #endregion
        #region Public Methods
        public override void Login(bool IsChecked)
        {
            IsBusy = true;
            if (IsChecked)
            {
                Preferences.Set(Keys.UserNameKey, UserName);
                Preferences.Set(Keys.PasswordKey, Password);
                Preferences.Set(Keys.EmailKey, Email);
            }
            else
                Preferences.Clear();
            fbd.SignInWithEmailAndPasswordAsync(Email, Password, OnComplete);
        }
        public override void Register()
        {
            IsBusy = true;
            fbd.CreateUserWithEmailAndPasswordAsync(Email, Password, UserName, OnComplete);
        }
        public override void ResetEmailPassword()
        {
             fbd.ResetEmailPasswordAsync(EmailForReset, OnResetComplete);
        }
        public override string GetFirebaseErrorMessage(string msg)
        {
            return msg.Contains(Strings.Reason) ?
                msg.Contains(Strings.EmailExists) ? Strings.EmailExistsmsg :
                msg.Contains(Strings.InvalidEmailAddress) ? Strings.InvalidEmailAddressmsg :
                msg.Contains(Strings.WeakPassword) ? Strings.WeakPasswordmsg :
                msg.Contains(Strings.UserNotFound) ? Strings.UserNotFoundmsg : 
                Strings.UnknownError : Strings.UnknownError;
        }
        public override bool CanLogIn()
        {
            return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Email);
        }
        public override bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Email);
        }
        #endregion
        #region Private Methods
        protected override void OnResetComplete(Task task)
        {
        }
        protected override void OnComplete(Task task)
        {
            IsBusy = false;
            if (task.IsCompletedSuccessfully)
            {
                SaveToPreferences();
                OnAuthComplete?.Invoke(this, EventArgs.Empty);
            }
            else if (task.Exception != null)
            {
                string msg = task.Exception.Message;
                ShowAlert(GetFirebaseErrorMessage(msg));
            }
            else
                ShowAlert(Strings.CreateUserError);
        }
        protected override void ShowAlert(string msg)
        {
            ShowToastAlert?.Invoke(this, msg);
        }
        protected override void SaveToPreferences()
        {
            Preferences.Set(Keys.UserNameKey, UserName);
            Preferences.Set(Keys.PasswordKey, Password);
            Preferences.Set(Keys.EmailKey, Email);
        }
        #endregion
    }
}
