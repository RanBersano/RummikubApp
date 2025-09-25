using RummikubApp.ViewModels;

namespace RummikubApp.Views;

public partial class LogInPage : ContentPage
{
	public LogInPage()
	{
        InitializeComponent();
		BindingContext = new LogInPageVM();
	}
}