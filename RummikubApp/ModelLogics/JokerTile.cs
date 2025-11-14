namespace RummikubApp.ModelLogics
{
    public class JokerTile : Tile
    {
        public JokerTile() : base(color: 0, number: 0) { }

        public override string ToString()
        {
            return "Joker";
        }
    }
}
