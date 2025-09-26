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
            Page page = user.IsRegistered ? new LogInPage() : new RegisterPage();
            MainPage = page;
        }
    }
}
