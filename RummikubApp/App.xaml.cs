using RummikubApp.ModelLogics;
using RummikubApp.Views;

namespace RummikubApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            User user = new();
            MainPage = user.IsRegistered ? new LogInPage() : new RegisterPage();
        }
    }
}
