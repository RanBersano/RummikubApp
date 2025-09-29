using RummikubApp.ViewModels;

namespace RummikubApp.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
        BindingContext = new RegisterPageVM();
    }
}