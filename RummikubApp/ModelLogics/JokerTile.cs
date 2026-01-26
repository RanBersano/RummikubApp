namespace RummikubApp.ModelLogics
{
    public class JokerTile : Tile
    {
        public JokerTile() : base(colorIndex: ColorIndexes.Orange, value: 0) { }
        public override string ToString()
        {
            return "Joker";
        }
    }
}
