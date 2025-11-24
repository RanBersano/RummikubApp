using RummikubApp.Models; 

namespace RummikubApp.ModelLogics
{
    public class Board : BoardModel
    {
        public Board()
        {
            Tiles = new List<Tile>();
        }

        public void AddTile(Tile tile)
        {
            Tiles.Add(tile);
        }
    }
}
