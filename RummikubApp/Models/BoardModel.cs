using Plugin.CloudFirestore.Attributes;

namespace RummikubApp.Models
{
    public abstract class BoardModel
    {
        #region Properties
        public TileData[] Tiles { get; protected set; } = [];
        public int SelectedIndex { get; protected set; } = -1;
        [Ignored]
        public int Capacity { get; protected set; } = 18;
        #endregion
        #region Public Methods
        public abstract TileData[] ExportToArray();
        public abstract void LoadFromArray(TileData[] tiles);
        public abstract void EnsureCapacity();
        public abstract void ClearSelection();
        public abstract bool HandleTap(int index);
        public abstract bool PlaceTileInFirstEmpty(TileData tile);
        public abstract int FindFirstEmptyTileIndex();
        #endregion
    }
}