using RummikubApp.ViewModels;

namespace RummikubApp.Views;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
        InitializeComponent();
        BindingContext = new HomePageVM();
    }
}