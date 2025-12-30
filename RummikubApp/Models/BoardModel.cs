using Plugin.CloudFirestore.Attributes;

namespace RummikubApp.Models
{
    public abstract class BoardModel
    {
        public int Capacity { get; protected set; } = 18;
        public TileData[] Tiles { get; protected set; } = new TileData[0];
        [Ignored]
        public int SelectedIndex { get; protected set; } = -1;
        public abstract void LoadFromArray(TileData[] tiles);
        public abstract TileData[] ExportToArray();
        public abstract void EnsureCapacity();
        public abstract bool HandleTap(int index);
        public abstract void ClearSelection();
        public abstract int FindFirstEmptyTileIndex();
        public abstract bool PlaceTileInFirstEmpty(TileData tile);
    }
}