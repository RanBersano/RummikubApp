using RummikubApp.Models;
namespace RummikubApp.ModelLogics
{
    internal class Game : GameModel
    {
        internal Game(GameSize selectedGameSize)
        {
            HostName = new User().UserName;
            RowSize = selectedGameSize.Size;
            Created = DateTime.Now;
        }
        internal Game()
        {
        }
        public override void SetDocument(Action<System.Threading.Tasks.Task> OnComplete)
        {
            Id = fbd.SetDocument(this, Keys.GamesCollection, Id, OnComplete);
        }
    }
}
