using Plugin.CloudFirestore.Attributes;

namespace RummikubApp.Models
{
    public abstract class BoardModel
    {
        public int Capacity { get; protected set; } = 18;

        public TileData[] Slots { get; protected set; } = new TileData[0];

        [Ignored]
        public int SelectedIndex { get; protected set; } = -1;

        public abstract void LoadFromArray(TileData[] slots);
        public abstract TileData[] ExportToArray();

        public abstract void EnsureCapacity();

        // Tap/Swap
        public abstract bool HandleTap(int index);
        public abstract void ClearSelection();

        // Place tile into first empty slot
        public abstract int FindFirstEmptySlotIndex();
        public abstract bool PlaceTileInFirstEmpty(TileData tile);
    }
}