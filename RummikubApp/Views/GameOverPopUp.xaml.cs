using CommunityToolkit.Maui.Views;
using RummikubApp.ViewModels;
namespace RummikubApp.Views
{
    public partial class GameOverPopUp : Popup
    {
        public GameOverPopUp(string title, string message)
        {
            InitializeComponent();
            BindingContext = new GameOverPopUpVM(this, title, message);
        }
    }
}
