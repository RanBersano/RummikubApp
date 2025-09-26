using RummikubApp.ViewModels;

namespace RummikubApp.Views;

public partial class Register : ContentPage
{
	public Register()
	{
		InitializeComponent();
        BindingContext = new RegisterPageVM();
    }
}