using CommunityToolkit.Maui.Alerts;
using Plugin.CloudFirestore;
using RummikubApp.Models;
namespace RummikubApp.ModelLogics
{
    public class Games: GamesModel
    {
        public void AddGame()
        {
            IsBusy = true;
            currentGame = new(SelectedGameSize);
            currentGame.IsHost = true;
            currentGame.SetDocument(OnComplete);
        }
        private void OnComplete(Task task)
        {
            IsBusy = false;
            OnGameAdded?.Invoke(this, currentGame!);
        }
        public Games()
        {

        }
        public void AddSnapshotListener()
        {
            ilr = fbd.AddSnapshotListener(Keys.GamesCollection, OnChange!);
        }
        public void RemoveSnapshotListener()
        {
            ilr?.Remove();
        }
        private void OnChange(IQuerySnapshot snapshot, Exception error)
        {
            fbd.GetDocumentsWhereEqualTo(Keys.GamesCollection, nameof(GameModel.IsFull), false, OnComplete);
        }

        private void OnComplete(IQuerySnapshot qs)
        {
            GamesList!.Clear();
            foreach (IDocumentSnapshot ds in qs.Documents)
            {
                Game? game = ds.ToObject<Game>();
                if (game != null)
                {
                    game.Id = ds.Id;
                    GamesList.Add(game);
                }
            }
            OnGamesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
