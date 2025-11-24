using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    public class Player : PlayerModel
    {
        public Player(string name)
        {
            Name = name;
            Board = new Board();
        }

        public void DrawFromDeck(Deck deck)
        {
            Tile tile = (Tile)deck.DrawTile();
            if (tile != null)
            {
                Board.AddTile(tile);
            }
        }
    }
}
