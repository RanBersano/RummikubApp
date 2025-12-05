using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    public class Deck : DeckModel
    {
        public Deck()
        {
            Tiles = new List<Tile>();
            CreateTiles();
            Shuffle();
        }

        private void CreateTiles()
        {
            // 2 קבוצות של 1–13 לכל צבע
            foreach (TileModel.Colors color in Enum.GetValues(typeof(TileModel.Colors)))
            {
                for (int n = 1; n <= 13; n++)
                {
                    Tiles.Add(new Tile(color, n));
                    Tiles.Add(new Tile(color, n));
                }
            }

            // 2 ג'וקרים
            Tiles.Add(new Tile());
            Tiles.Add(new Tile());
        }

        private void Shuffle()
        {
            Random rnd = new Random();
            Tiles = Tiles.OrderBy(x => rnd.Next()).ToList();
        }

        public TileModel DrawTile()
        {
            if (Tiles.Count == 0)
                return null;

            TileModel t = Tiles[0];
            Tiles.RemoveAt(0);
            return t;
        }
    }
}
