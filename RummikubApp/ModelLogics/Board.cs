using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    public class Board : BoardModel
    {
        public List<List<Tile>> Sets { get; } = new List<List<Tile>>();

        public void AddSet(List<Tile> set)
        {
            Sets.Add(set);
        }
    }
}
