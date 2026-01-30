using RummikubApp.ModelLogics;
using RummikubApp.ViewModels;

namespace RummikubApp.Views;

public partial class GamePage : ContentPage
{
    private readonly GamePageVM gpVM;
	public GamePage(Game game)
	{
		InitializeComponent();
        gpVM = new GamePageVM(game, grdDeck);
        BindingContext = gpVM;
        HandHost.Children.Add(new PlayerHandCV(game));
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        gpVM.AddSnapshotListener();
    }

    protected override void OnDisappearing()
    {
        gpVM.RemoveSnapshotListener();
        base.OnDisappearing();
    }
}