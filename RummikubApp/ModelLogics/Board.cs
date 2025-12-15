using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    public class Board : BoardModel
    {
        public Board()
        {
            Capacity = 18;
            Slots = new List<TileData>();
            SelectedIndex = -1;
        }

        public Board(List<TileData> existingSlots)
        {
            Capacity = 18;
            Slots = existingSlots ?? new List<TileData>();
            SelectedIndex = -1;
            EnsureCapacity();
        }

        public override void EnsureCapacity()
        {
            if (Slots == null)
            {
                Slots = new List<TileData>();
            }

            while (Slots.Count < Capacity)
            {
                TileData empty = new TileData();
                empty.IsEmptySlot = true;
                empty.IsJoker = false;
                empty.Color = 0;
                empty.Number = 0;
                Slots.Add(empty);
            }

            if (Slots.Count > Capacity)
            {
                Slots = Slots.Take(Capacity).ToList();
            }
        }

        public override bool TrySelect(int index)
        {
            if (index < 0 || index >= Slots.Count)
            {
                return false;
            }

            if (Slots[index].IsEmptySlot)
            {
                return false;
            }

            SelectedIndex = index;
            return true;
        }

        public override bool TryClearSelection()
        {
            if (SelectedIndex == -1)
            {
                return false;
            }

            SelectedIndex = -1;
            return true;
        }

        public override bool TrySwapWithSelected(int index)
        {
            if (SelectedIndex == -1)
            {
                return false;
            }

            if (index < 0 || index >= Slots.Count)
            {
                return false;
            }

            if (SelectedIndex < 0 || SelectedIndex >= Slots.Count)
            {
                SelectedIndex = -1;
                return false;
            }

            if (SelectedIndex == index)
            {
                SelectedIndex = -1;
                return true;
            }

            TileData first = Slots[SelectedIndex];
            TileData second = Slots[index];

            Slots[SelectedIndex] = second;
            Slots[index] = first;

            SelectedIndex = -1;
            return true;
        }

        public override int FindFirstEmptySlotIndex()
        {
            for (int i = 0; i < Slots.Count; i++)
            {
                if (Slots[i].IsEmptySlot)
                {
                    return i;
                }
            }

            return -1;
        }

        public override bool TryPlaceTileInFirstEmpty(TileData tile)
        {
            if (tile == null)
            {
                return false;
            }

            int emptyIndex = FindFirstEmptySlotIndex();
            if (emptyIndex == -1)
            {
                return false;
            }

            tile.IsEmptySlot = false;
            Slots[emptyIndex] = tile;
            return true;
        }
    }
}
