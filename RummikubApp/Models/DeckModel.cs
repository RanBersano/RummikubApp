namespace RummikubApp.Models
{
    public abstract class DeckModel
    {
        #region Properties
        public TileData[] Tiles { get; protected set; } = [];
        public abstract int Count { get; }
        #endregion
        #region Public Methods
        public abstract TileData[] DealTiles(int count);
        public abstract TileData? DrawTileData();
        public abstract TileData[] ExportToArray();
        public abstract void BuildFullDeck();
        public abstract void Shuffle();
        public abstract void LoadFromArray(TileData[] tiles);
        #endregion
    }
}
