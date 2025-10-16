using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    internal class User : UserModel
    {
        public override void LogIn()
        {
            Preferences.Set(Keys.UserNameKey, UserName);
            Preferences.Set(Keys.PasswordKey, Password);
            Preferences.Set(Keys.EmailKey, Email);
        }
        public override void Register()
        {
            fbd.CreateUserWithEmailAndPasswordAsync(Email, Password, UserName, OnComplete);
        }

        private void OnComplete(Task task)
        {
           MainThread.BeginInvokeOnMainThread(async () =>
           {
              if (task.IsCompletedSuccessfully)
              { 
                 SaveToPreferences();
              }
              else
              {
                 await Application.Current.MainPage.DisplayAlert(
                        Strings.RegistrationFailed,
                        Strings.Error,
                        Strings.Ok
                 );
              }
           });
        }

        private void SaveToPreferences()
        {
            Preferences.Set(Keys.UserNameKey, UserName);
            Preferences.Set(Keys.PasswordKey, Password);
            Preferences.Set(Keys.EmailKey, Email);
        }

        public User()
        {
            UserName = Preferences.Get(Keys.UserNameKey, string.Empty);
            Password = Preferences.Get(Keys.PasswordKey, string.Empty);
            Email = Preferences.Get(Keys.EmailKey, string.Empty);
        }
    }
}
