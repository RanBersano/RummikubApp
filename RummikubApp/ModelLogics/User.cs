using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    internal class User : UserModel
    {
        public override void LogIn()
        {
            fbd.SignInWithEmailAndPasswordAsync(Email, Password, OnComplete);
            Preferences.Set(Keys.UserNameKey, UserName);
            Preferences.Set(Keys.PasswordKey, Password);
            Preferences.Set(Keys.EmailKey, Email);
        }
        public async Task ResetPassword()
        {
            await fbd.SendPasswordResetEmailAsync(Email, OnCompleteSendEmail);
        }
        private async Task OnCompleteSendEmail(Task task)
        {
            if (task.IsCompletedSuccessfully)
            {
                await Shell.Current.DisplayAlert(Strings.ResetPWTitle, Strings.ResetPWMessage, Strings.ResetPWButton);
            }
            else
            {
                await Shell.Current.DisplayAlert(Strings.ResetPWErrorTitle, "errorMessage", Strings.ResetPWErrorButton);
            }
        }
        public override void Register()
        {
            fbd.CreateUserWithEmailAndPasswordAsync(Email, Password, UserName, OnComplete);
        }
        private void OnComplete(Task task)
        {
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
        public override string GetFirebaseErrorMessage(string msg)
        {
            return msg.Contains(Strings.Reason) ?
                msg.Contains(Strings.EmailExists) ? Strings.EmailExistsmsg :
                msg.Contains(Strings.InvalidEmailAddress) ? Strings.InvalidEmailAddressmsg :
                msg.Contains(Strings.WeakPassword) ? Strings.WeakPasswordmsg :
                msg.Contains(Strings.UserNotFound) ? Strings.UserNotFoundmsg : 
                Strings.UnknownError : Strings.UnknownError;
        }
        private static void ShowAlert(string msg)
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
               Toast.Make(msg, ToastDuration.Long).Show();
            });
        }
        private void SaveToPreferences()
        {
            Preferences.Set(Keys.UserNameKey, UserName);
            Preferences.Set(Keys.PasswordKey, Password);
            Preferences.Set(Keys.EmailKey, Email);
        }
        public override bool CanLogIn()
        {
            return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Email);
        }
        public override bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Email);
        }
        public User()
        {
            UserName = Preferences.Get(Keys.UserNameKey, string.Empty);
            Password = Preferences.Get(Keys.PasswordKey, string.Empty);
            Email = Preferences.Get(Keys.EmailKey, string.Empty);
        }
    }
}
