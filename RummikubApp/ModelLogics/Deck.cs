using System;
using System.Collections.Generic;
using System.Linq;

namespace RummikubApp.Models
{
    public class Deck : DeckModel
    {
        public Deck()
        {
            Tiles = new List<TileData>();
            BuildFullDeck();
            Shuffle();
        }

        public Deck(List<TileData> existingTiles)
        {
            Tiles = existingTiles ?? new List<TileData>();
        }

        private void BuildFullDeck()
        {
            Tiles.Clear();

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
                        IsJoker = false
                    };

                    TileData tile2 = new TileData
                    {
                        Color = (int)color,
                        Number = n,
                        IsJoker = false
                    };

                    Tiles.Add(tile1);
                    Tiles.Add(tile2);
                }
            }

            TileData joker1 = new TileData
            {
                Color = 0,
                Number = 0,
                IsJoker = true
            };

            TileData joker2 = new TileData
            {
                Color = 0,
                Number = 0,
                IsJoker = true
            };

            Tiles.Add(joker1);
            Tiles.Add(joker2);
        }

        private void Shuffle()
        {
            Random rnd = new Random();
            List<TileData> shuffled = Tiles
                .OrderBy(tile => rnd.Next())
                .ToList();
            Tiles = shuffled;
        }

        public List<TileData> DealTiles(int count)
        {
            List<TileData> hand = new List<TileData>();

            for (int i = 0; i < count; i++)
            {
                if (Tiles.Count == 0)
                {
                    break;
                }

                TileData top = Tiles[0];
                Tiles.RemoveAt(0);
                hand.Add(top);
            }

            return hand;
        }
    }
}