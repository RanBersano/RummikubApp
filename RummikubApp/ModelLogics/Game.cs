using RummikubApp.Models;
namespace RummikubApp.ModelLogics
{
    public class Game : GameModel
    {
        public override string OpponentName => IsHost ? GuestName : HostName;
        public Game(GameSize selectedGameSize)
        {
            HostName = new User().UserName;
            Players = selectedGameSize.Size;
            Created = DateTime.Now;
        }
        public Game()
        {
        }
        public override void SetDocument(Action<System.Threading.Tasks.Task> OnComplete)
        {
            Id = fbd.SetDocument(this, Keys.GamesCollection, Id, OnComplete);
        }
    }
}
