using CommunityToolkit.Maui.Alerts;
using Plugin.CloudFirestore;
using RummikubApp.Models;
namespace RummikubApp.ModelLogics
{
    internal class Games: GamesModel
    {
        internal void AddGame()
        {
            IsBusy = true;
            Game game = new(SelectedGameSize);
            game.SetDocument(OnComplete);
        }
        private void OnComplete(Task task)
        {
            IsBusy = false;
            OnGameAdded?.Invoke(this, task.IsCompletedSuccessfully);
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
