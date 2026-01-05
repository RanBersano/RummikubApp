using RummikubApp.Models;
namespace RummikubApp.ModelLogics
{
    public class Board : BoardModel
    {
        public Board()
        {
            Capacity = 18;
            Tiles = [];
            SelectedIndex = -1;
            EnsureCapacity();
        }
        public Board(TileData[] existingTiles)
        {
            Capacity = 18;
            LoadFromArray(existingTiles);
            SelectedIndex = -1;
            EnsureCapacity();
        }
        public override void LoadFromArray(TileData[] tiles)
        {
            if (tiles == null)
            {
                Tiles = [];
                return;
            }
            TileData[] copy = new TileData[tiles.Length];
            for (int i = 0; i < tiles.Length; i++)
            {
                copy[i] = tiles[i];
            }
            Tiles = copy;
        }
        public override TileData[] ExportToArray()
        {
            TileData[] copy = new TileData[Tiles.Length];
            for (int i = 0; i < Tiles.Length; i++)
                copy[i] = Tiles[i];
            return copy;
        }

        public override void EnsureCapacity()
        {
            if (Tiles == null)
                Tiles = [];
            if (Tiles.Length == Capacity)
                return;
            TileData[] newTiles = new TileData[Capacity];
            for (int i = 0; i < Tiles.Length; i++)
                newTiles[i] = Tiles[i];
            for (int i = Tiles.Length; i < Capacity; i++)
            {
                newTiles[i] = new TileData
                {
                    IsEmptyTile = true,
                    IsJoker = false,
                    Color = 0,
                    Number = 0
                };
            }
            Tiles = newTiles;
        }
        public override void ClearSelection()
        {
            SelectedIndex = -1;
        }
        public override bool HandleTap(int index)
        {
            if (index < 0 || index >= Tiles.Length)
                return false;
            if (SelectedIndex == -1)
            {
                if (Tiles[index].IsEmptyTile)
                    return false;
                SelectedIndex = index;
                return true;
            }
            if (SelectedIndex == index)
            {
                SelectedIndex = -1;
                return true;
            }
            TileData tmp = Tiles[SelectedIndex];
            Tiles[SelectedIndex] = Tiles[index];
            Tiles[index] = tmp;
            SelectedIndex = -1;
            return true;
        }
        public override int FindFirstEmptyTileIndex()
        {
            for (int i = 0; i < Tiles.Length; i++)
                if (Tiles[i].IsEmptyTile)
                    return i;
            return -1;
        }
        public override bool PlaceTileInFirstEmpty(TileData tile)
        {
            if (tile == null)
                return false;
            int idx = FindFirstEmptyTileIndex();
            if (idx == -1)
                return false;
            tile.IsEmptyTile = false;
            Tiles[idx] = tile;
            return true;
        }
    }
}