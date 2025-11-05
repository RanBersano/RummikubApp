using Plugin.CloudFirestore.Attributes;
using RummikubApp.ModelLogics;
namespace RummikubApp.Models
{
    internal abstract class GameModel
    {
        protected FbData fbd = new();
        [Ignored]
        public string Id { get; set; } = string.Empty;
        public string HostName { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public int RowSize { get; set; }
        public bool IsFull { get; set; }
        public abstract void SetDocument(Action<System.Threading.Tasks.Task> OnComplete);
    }
}
