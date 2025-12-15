using Plugin.CloudFirestore.Attributes;
using RummikubApp.ModelLogics;

namespace RummikubApp.Models
{
    public abstract class BoardModel
    {
        public int Capacity { get; set; } = 18; // להחליף שם

        // זה מה שנשמר בפיירסטור (רשימת סלוטים)
        public List<TileData> Slots { get; set; } = new List<TileData>();

        // מצב בחירה - לא נשמר בפיירסטור (UI-STATE)
        [Ignored]
        public int SelectedIndex { get; set; } = -1;

        public abstract void EnsureCapacity();
        public abstract bool TrySelect(int index);
        public abstract bool TryClearSelection();
        public abstract bool TrySwapWithSelected(int index);
        public abstract int FindFirstEmptySlotIndex();
        public abstract bool TryPlaceTileInFirstEmpty(TileData tile);
    }
}

