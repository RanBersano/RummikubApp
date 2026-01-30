using RummikubApp.ModelLogics;
using RummikubApp.ViewModels;

namespace RummikubApp.Views;

public partial class PlayerHandCV : ContentView
{
    public PlayerHandCV(Game game)
    {
        InitializeComponent();
        BindingContext = new PlayerHandVM(game);
    }
}
