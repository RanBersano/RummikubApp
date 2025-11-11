using Plugin.CloudFirestore;
using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    public class Game : GameModel
    { 
        public Game(GameSize selectedGameSize)
        {
            HostName = new User().UserName;
            Players = selectedGameSize.Size;
            Created = DateTime.Now;
            CurrentNumOfPlayers = 1;
        }
        public Game()
        {
        }
        public string GetOtherPlayerName(int index)
        {
            string[] others = new string[]
            {
                HostName,
                PlayerName2,
                PlayerName3,
                PlayerName4
            };

            List<string> validNames = new List<string>();
            for (int i = 0; i < others.Length; i++)
            {
                string name = others[i];
                if (!string.IsNullOrWhiteSpace(name) &&
                    !string.Equals(name, MyName, StringComparison.Ordinal))
                {
                    validNames.Add(name);
                }
            }

            if (index < validNames.Count)
                return validNames[index];
            else
                return string.Empty;
        }

        public override void SetDocument(Action<Task> OnComplete)
        {
            Id = fbd.SetDocument(this, Keys.GamesCollection, Id, OnComplete);
        }
        public void UpdateGuestUser(Action<Task> onComplete)
        {
            if (IsFull)
            {
                Task failedTask = Task.FromException(
                    new InvalidOperationException(Strings.GameFull));
                onComplete(failedTask);
                return;
            }
            Dictionary<string, object> updates = new Dictionary<string, object>();

            if (string.IsNullOrEmpty(PlayerName2))
            {
                PlayerName2 = MyName;
                updates[nameof(PlayerName2)] = PlayerName2;
            }
            else if (Players >= 3 && string.IsNullOrEmpty(PlayerName3))
            {
                PlayerName3 = MyName;
                updates[nameof(PlayerName3)] = PlayerName3;
            }
            else if (Players == 4 && string.IsNullOrEmpty(PlayerName4))
            {
                PlayerName4 = MyName;
                updates[nameof(PlayerName4)] = PlayerName4;
            }
            else
            {
                Task failedTask = Task.FromException(
                    new InvalidOperationException(Strings.GameFull));
                onComplete(failedTask);
                return;
            }

            CurrentNumOfPlayers++;
            updates[nameof(CurrentNumOfPlayers)] = CurrentNumOfPlayers;

            if (CurrentNumOfPlayers >= Players)
            {
                IsFull = true;
                updates[nameof(IsFull)] = true;
            }

            fbd.UpdateFields(Keys.GamesCollection, Id, updates, onComplete);
        }
        public override void AddSnapshotListener()
        {
            ilr = fbd.AddSnapshotListener(Keys.GamesCollection, Id, OnChange);
        }
        public override void RemoveSnapshotListener()
        {
            ilr?.Remove();
            DeleteDocument(OnComplete);
        }
        private void OnComplete(Task task)
        {
            if(task.IsCompletedSuccessfully)
                OnGameDeleted?.Invoke(this, EventArgs.Empty);
        }
        private void OnChange(IDocumentSnapshot? snapshot, Exception? error)
        {
            if (error != null || snapshot == null || !snapshot.Exists)
            {
                return;
            }

            Game? updatedGame = snapshot.ToObject<Game>();
            if (updatedGame == null)
            {
                return;
            }
            Players = updatedGame.Players;
            IsFull = updatedGame.IsFull;
            CurrentNumOfPlayers = updatedGame.CurrentNumOfPlayers;
            HostName = updatedGame.HostName;
            PlayerName2 = updatedGame.PlayerName2;
            PlayerName3 = updatedGame.PlayerName3;
            PlayerName4 = updatedGame.PlayerName4;

            OnGameChanged?.Invoke(this, EventArgs.Empty);
        }
        public override void DeleteDocument(Action<Task> onComplete)
        {
            fbd.DeleteDocument(Keys.GamesCollection, Id, onComplete);
        }
    }
}
