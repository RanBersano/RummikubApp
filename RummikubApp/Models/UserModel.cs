namespace RummikubApp.Models
{
    internal abstract class UserModel
    {
        public bool IsRegistered => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Email);
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public abstract void LogIn();
        public abstract void Register();
    }
}
