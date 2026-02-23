namespace RummikubApp.ModelLogics
{
    public partial class JokerTile : Tile
    {
        #region Constructor
        public JokerTile() : base(colorIndex: ColorIndexes.Orange, value: 0) { }
        #endregion
        #region Public Methods
        public override string ToString()
        {
            return "Joker";
        }
        #endregion
    }
}
