namespace RummikubApp.ModelLogics
{
    public partial class JokerTile : Tile
    {
        public JokerTile() : base(colorIndex: ColorIndexes.Orange, value: 0) { }
        public override string ToString()
        {
            return "Joker";
        }
    }
}
