using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Plugin.CloudFirestore;
using RummikubApp.Models;
namespace RummikubApp.ModelLogics
{
    public class Games: GamesModel
    {
        public override void AddGame()
        {
            IsBusy = true;
            currentGame = new(SelectedGameSize)
            {
                IsHostUser = true
            };
            currentGame.OnGameDeleted += OnGameDeleted;
            currentGame.SetDocument(OnComplete);
        }
        protected override void OnGameDeleted(object? sender, EventArgs e)
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Toast.Make(Strings.GameDeleted, ToastDuration.Long).Show();
            });
        }
        protected override void OnComplete(Task task)
        {
            IsBusy = false;
            OnGameAdded?.Invoke(this, currentGame!);
        }
        public Games()
        {
        }
        public override void AddSnapshotListener()
        {
            ilr = fbd.AddSnapshotListener(Keys.GamesCollection, OnChange!);
        }
        public override void RemoveSnapshotListener()
        {
            ilr?.Remove();
        }
        protected override void OnChange(IQuerySnapshot snapshot, Exception error)
        {
            fbd.GetDocumentsWhereEqualTo(Keys.GamesCollection, nameof(GameModel.IsFull), false, OnComplete);
        }
        protected override void OnComplete(IQuerySnapshot qs)
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
