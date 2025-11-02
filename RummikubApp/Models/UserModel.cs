using RummikubApp.ModelLogics;
namespace RummikubApp.Models
{
    internal abstract class UserModel
    {
        protected FbData fbd = new();
        public EventHandler? OnAuthComplete;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ResetEmail { get; set; } = string.Empty;
        public bool IsRegistered => (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Email));
        public abstract void Login(bool IsChecked);
        public abstract void Register();
        public abstract bool CanLogIn();
        public abstract bool CanRegister();
        public abstract Task ResetPassword();
        public abstract string GetFirebaseErrorMessage(string msg);

    }
}
