using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    public class Board : BoardModel
    {
        public Board()
        {
            Capacity = 18;
            Slots = new TileData[0];
            SelectedIndex = -1;
            EnsureCapacity();
        }

        public Board(TileData[] existingSlots)
        {
            Capacity = 18;
            LoadFromArray(existingSlots);
            SelectedIndex = -1;
            EnsureCapacity();
        }

        public override void LoadFromArray(TileData[] slots)
        {
            if (slots == null)
            {
                Slots = new TileData[0];
                return;
            }

            TileData[] copy = new TileData[slots.Length];
            for (int i = 0; i < slots.Length; i++)
            {
                copy[i] = slots[i];
            }
            Slots = copy;
        }

        public override TileData[] ExportToArray()
        {
            TileData[] copy = new TileData[Slots.Length];
            for (int i = 0; i < Slots.Length; i++)
            {
                copy[i] = Slots[i];
            }
            return copy;
        }

        public override void EnsureCapacity()
        {
            if (Slots == null)
            {
                Slots = new TileData[0];
            }

            if (Slots.Length == Capacity)
            {
                return;
            }

            TileData[] newSlots = new TileData[Capacity];

            int copyLen = Slots.Length;
            if (copyLen > Capacity)
            {
                copyLen = Capacity;
            }

            for (int i = 0; i < copyLen; i++)
            {
                newSlots[i] = Slots[i];
            }

            for (int i = copyLen; i < Capacity; i++)
            {
                newSlots[i] = new TileData
                {
                    IsEmptySlot = true,
                    IsJoker = false,
                    Color = 0,
                    Number = 0,
                    IsSelected = false
                };
            }

            Slots = newSlots;
        }

        public override void ClearSelection()
        {
            for (int i = 0; i < Slots.Length; i++)
            {
                Slots[i].IsSelected = false;
            }
            SelectedIndex = -1;
        }

        public override bool HandleTap(int index)
        {
            if (index < 0 || index >= Slots.Length)
            {
                return false;
            }

            if (SelectedIndex == -1)
            {
                if (Slots[index].IsEmptySlot)
                {
                    return false;
                }

                ClearSelection();
                SelectedIndex = index;
                Slots[index].IsSelected = true;
                return true;
            }

            if (SelectedIndex == index)
            {
                ClearSelection();
                return true;
            }

            TileData first = Slots[SelectedIndex];
            TileData second = Slots[index];

            first.IsSelected = false;

            Slots[SelectedIndex] = second;
            Slots[index] = first;

            SelectedIndex = -1;
            return true;
        }

        public override int FindFirstEmptySlotIndex()
        {
            for (int i = 0; i < Slots.Length; i++)
            {
                if (Slots[i].IsEmptySlot)
                {
                    return i;
                }
            }
            return -1;
        }

        public override bool PlaceTileInFirstEmpty(TileData tile)
        {
            if (tile == null)
            {
                return false;
            }

            int idx = FindFirstEmptySlotIndex();
            if (idx == -1)
            {
                return false;
            }

            tile.IsEmptySlot = false;
            tile.IsSelected = false;
            Slots[idx] = tile;
            return true;
        }
    }
}