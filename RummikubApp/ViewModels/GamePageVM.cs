using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using RummikubApp.ModelLogics;
using RummikubApp.Models;

namespace RummikubApp.ViewModels
{
    public class GamePageVM : ObserableObject
    {
        private readonly Game game;
        public string MyName => game.MyName;
        public string OpponentName => game.OpponentName;
        public GamePageVM(Game game)
        {
            this.game = game;
            if (!game.IsHost)
            {
                game.GuestName = MyName;
                game.IsFull = true;
                game.SetDocument(OnComplete);
            }
        }

        private void OnComplete(Task task)
        {
            if (!task.IsCompletedSuccessfully)
                Toast.Make(Strings.JoinGameErr, ToastDuration.Long).Show();
        }
    }
}
