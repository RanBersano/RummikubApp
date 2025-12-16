namespace RummikubApp.Models
{
    public abstract class DeckModel
    {
        public TileData[] Tiles { get; protected set; } = new TileData[0];

        public abstract int Count { get; }

        public abstract void BuildFullDeck();
        public abstract void Shuffle();
        public abstract TileData[] DealTiles(int count);
        public abstract TileData? DrawTileData();

        public abstract void LoadFromArray(TileData[] tiles);
        public abstract TileData[] ExportToArray();
    }
}
