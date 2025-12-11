namespace RummikubApp.Models
{
    public class DeckModel
    {
        public List<TileData> Tiles { get; set; }

        public DeckModel()
        {
            Tiles = new List<TileData>();
        }
    }
}