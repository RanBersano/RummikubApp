using RummikubApp.ModelLogics;
using RummikubApp.ViewModels;

namespace RummikubApp.Views;

public partial class GamePage : ContentPage
{
	public GamePage(Game game)
	{
		InitializeComponent();
		BindingContext = new GamePageVM(game);
    }
}