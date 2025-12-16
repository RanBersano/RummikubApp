using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    public class Deck : DeckModel
    {
        private readonly Random rnd = new Random();

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
            {
                Tiles = new TileData[0];
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
            {
                copy[i] = Tiles[i];
            }
            return copy;
        }

        public override void BuildFullDeck()
        {
            // 4 צבעים * 13 מספרים * 2 עותקים = 104 + 2 ג'וקרים = 106
            int total = (4 * 13 * 2) + 2;
            Tiles = new TileData[total];

            int index = 0;
            Array colors = Enum.GetValues(typeof(TileModel.Colors));
            for (int i = 0; i < colors.Length; i++)
            {
                TileModel.Colors color = (TileModel.Colors)colors.GetValue(i)!;

                for (int n = 1; n <= 13; n++)
                {
                    TileData tile1 = new TileData
                    {
                        Color = (int)color,
                        Number = n,
                        IsJoker = false,
                        IsEmptySlot = false,
                        IsSelected = false
                    };

                    TileData tile2 = new TileData
                    {
                        Color = (int)color,
                        Number = n,
                        IsJoker = false,
                        IsEmptySlot = false,
                        IsSelected = false
                    };

                    Tiles[index] = tile1;
                    index++;
                    Tiles[index] = tile2;
                    index++;
                }
            }

            Tiles[index] = new TileData { Color = 0, Number = 0, IsJoker = true, IsEmptySlot = false, IsSelected = false };
            index++;
            Tiles[index] = new TileData { Color = 0, Number = 0, IsJoker = true, IsEmptySlot = false, IsSelected = false };
        }

        public override void Shuffle()
        {
            // Fisher-Yates
            for (int i = Tiles.Length - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);

                TileData temp = Tiles[i];
                Tiles[i] = Tiles[j];
                Tiles[j] = temp;
            }
        }

        public override TileData[] DealTiles(int count)
        {
            if (count <= 0 || Tiles.Length == 0)
            {
                return new TileData[0];
            }

            int take = count;
            if (take > Tiles.Length)
            {
                take = Tiles.Length;
            }

            TileData[] hand = new TileData[take];
            for (int i = 0; i < take; i++)
            {
                hand[i] = Tiles[i];
            }

            int remaining = Tiles.Length - take;
            TileData[] newDeck = new TileData[remaining];
            for (int i = 0; i < remaining; i++)
            {
                newDeck[i] = Tiles[take + i];
            }

            Tiles = newDeck;
            return hand;
        }

        public override TileData? DrawTileData()
        {
            if (Tiles.Length == 0)
            {
                return null;
            }

            TileData top = Tiles[0];

            int remaining = Tiles.Length - 1;
            TileData[] newDeck = new TileData[remaining];
            for (int i = 0; i < remaining; i++)
            {
                newDeck[i] = Tiles[i + 1];
            }

            Tiles = newDeck;
            return top;
        }
    }
}
