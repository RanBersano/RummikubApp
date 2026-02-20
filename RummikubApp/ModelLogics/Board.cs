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
                Tiles = [];
            else
            {
                TileData[] copy = new TileData[tiles.Length];
                for (int i = 0; i < tiles.Length; i++)
                    copy[i] = tiles[i];
                Tiles = copy;
            }
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
            Tiles ??= [];
            if (Tiles.Length != Capacity)
            {
                TileData[] newTiles = new TileData[Capacity];
                for (int i = 0; i < Tiles.Length; i++)
                    newTiles[i] = Tiles[i];
                for (int i = Tiles.Length; i < Capacity; i++)
                    newTiles[i] = new TileData
                    {
                        IsEmptyTile = true,
                        IsJoker = false,
                        ColorIndex = (int)TileModel.ColorIndexes.Orange,
                        Value = 0
                    };
                Tiles = newTiles;
            }
        }
        public override void ClearSelection()
        {
            SelectedIndex = -1;
        }
        public override bool HandleTap(int index)
        {
            bool result = true;
            if (index < 0 || index >= Tiles.Length)
                result = false;
            else if (SelectedIndex == -1)
            {
                if (Tiles[index].IsEmptyTile)
                    _ = false;
                SelectedIndex = index;
                result = true;
            }
            else if (SelectedIndex == index)
            {
                SelectedIndex = -1;
                result = true;
            }
            else
            {
                (Tiles[index], Tiles[SelectedIndex]) = (Tiles[SelectedIndex], Tiles[index]);
                SelectedIndex = -1;
            }
            return result;
        }
        public override int FindFirstEmptyTileIndex()
        {
            int result = -1;
            bool found = false;
            for (int i = 0; i < Tiles.Length; i++)
                if (Tiles[i].IsEmptyTile && !found)
                {
                    result = i;
                    found = true;
                }      
            return result;
        }
        public override bool PlaceTileInFirstEmpty(TileData tile)
        {
            bool result = false;
            if (tile != null)
            {
                int idx = FindFirstEmptyTileIndex();
                if (idx != -1)
                {
                    tile.IsEmptyTile = false;
                    Tiles[idx] = tile;
                    result = true;
                }
            }
            return result;
        }
    }
}