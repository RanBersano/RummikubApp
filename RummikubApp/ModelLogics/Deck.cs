using RummikubApp.Models;
namespace RummikubApp.ModelLogics
{
    public class Deck : DeckModel
    {
        private readonly Random rnd = new();
        public Deck()
        {
            BuildFullDeck();
            Shuffle();
        }
        public Deck(TileData[] existingTiles)
        {
            LoadFromArray(existingTiles);
        }
        public override int Count
        {
            get { return Tiles.Length; }
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
        public override void BuildFullDeck()
        {
            int total = (4 * 13 * 2) + 2;
            Tiles = new TileData[total];
            int index = 0;
            Array colors = Enum.GetValues(typeof(TileModel.ColorIndexes));
            for (int i = 0; i < colors.Length; i++)
            {
                TileModel.ColorIndexes color = (TileModel.ColorIndexes)colors.GetValue(i)!;
                for (int n = 1; n <= 13; n++)
                {
                    Tiles[index] = new TileData
                    {
                        ColorIndex = (int)color,
                        Value = n,
                        IsJoker = false,
                        IsEmptyTile = false
                    };
                    index++;
                    Tiles[index] = new TileData
                    {
                        ColorIndex = (int)color,
                        Value = n,
                        IsJoker = false,
                        IsEmptyTile = false
                    };
                    index++;
                }
            }
            Tiles[index] = new TileData { ColorIndex = 0, Value = 0, IsJoker = true, IsEmptyTile = false };
            index++;
            Tiles[index] = new TileData { ColorIndex = 0, Value = 0, IsJoker = true, IsEmptyTile = false };
        }
        public override void Shuffle()
        {
            for (int i = Tiles.Length - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                (Tiles[j], Tiles[i]) = (Tiles[i], Tiles[j]);
            }
        }
        public override TileData[] DealTiles(int count)
        {
            TileData[] hand;
            if (count <= 0 || Tiles.Length == 0)
                hand = [];
            else
            {
                int take = count;
                if (take > Tiles.Length)
                    take = Tiles.Length;
                hand = new TileData[take];
                for (int i = 0; i < take; i++)
                    hand[i] = Tiles[i];
                int remaining = Tiles.Length - take;
                TileData[] newDeck = new TileData[remaining];
                for (int i = 0; i < remaining; i++)
                    newDeck[i] = Tiles[take + i];
                Tiles = newDeck;
            }
            return hand;
        }
        public override TileData? DrawTileData()
        {
            TileData? result = null;
            if (Tiles.Length != 0)
            {
                result = Tiles[0];
                int remaining = Tiles.Length - 1;
                TileData[] newDeck = new TileData[remaining];
                for (int i = 0; i < remaining; i++)
                    newDeck[i] = Tiles[i + 1];
                Tiles = newDeck;
            }
            return result;
        }
    }
}