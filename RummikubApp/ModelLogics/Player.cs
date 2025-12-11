//using RummikubApp.Models;

//namespace RummikubApp.ModelLogics
//{
//    public class Player : PlayerModel
//    {
//        public Player(string name)
//        {
//            Name = name;
//            Board = new Board();
//        }

//        public Tile? DrawFromDeck(Deck deck)
//        {
//            TileModel model = deck.DrawTile();
//            if (model == null)
//                return null;
//            Tile? tile = model as Tile;
//            if (tile == null)
//                return null;
//            Board.AddTile(tile);
//            return tile;
//        }
//    }
//}
