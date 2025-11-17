using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    public class Deck : DeckModel
    {
        protected Tile[] deck = new Tile[106];
        //public List<Tile> Tiles { get; set; }

        //public Deck()
        //{
        //    Tiles = new List<Tile>();
        //    CreateTiles();
        //}

        //private void CreateTiles()
        //{
        //    foreach (Colors color in Enum.GetValues(typeof(Colors)))
        //    {
        //        for (int n = 1; n <= 13; n++)
        //        {
        //            Tiles.Add(new Tile(color, n));
        //            Tiles.Add(new Tile(color, n));  // שני עותקים
        //        }
        //    }

        //    // ג'וקרים
        //    Tiles.Add(new JokerTile());
        //    Tiles.Add(new JokerTile());
        //}
    }
}
