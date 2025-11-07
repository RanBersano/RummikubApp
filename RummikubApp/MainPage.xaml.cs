using RummikubApp.ViewModels;

namespace RummikubApp
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new HomePageVM();
        }

        
    }

}
