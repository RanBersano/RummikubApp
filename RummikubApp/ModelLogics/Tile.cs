using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    public partial class Tile : TileModel
    {
        #region Constructors
        public Tile(ColorIndexes colorIndex, int value) : base(colorIndex, value)
        {
            ColorIndex = colorIndex;
            Value = value;
            IsJoker = false;
        }
        public Tile() : base()
        {
            IsJoker = true;
            Value = 0;
            Source = Strings.Joker;
            Aspect = Aspect.AspectFit;
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Start, false);
            WidthRequest = 40;
        }
        public Tile(bool isEmptyTile) : base()
        {
            IsEmptyTile = isEmptyTile;
            IsJoker = false;
            Value = 0;
            Source = null;
            Aspect = Aspect.AspectFit;
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Start, false);
            WidthRequest = 40;
        }
        #endregion
        #region Public Methods
        public override Tile CreateEmptyTile()
        {
            Tile empty = new(true);
            return empty;
        }
        public override ImageSource? GetSourceFor(ColorIndexes color, int value)
        {
            ImageSource? result = null;
            if (value > 0)
            {
                string file = GetFileName(color, value);
                result = ImageSource.FromFile(file);
            }
            return result;
        }
        public override Tile FromTileData(TileData data)
        {
            _ = CreateEmptyTile();
            Tile result;
            if (data == null)
                result = CreateEmptyTile();
            else if (data.IsEmptyTile)
                result = CreateEmptyTile();
            else if (data.IsJoker)
                result = new Tile();
            else
            {
                result = new((ColorIndexes)data.ColorIndex, data.Value)
                {
                    IsEmptyTile = false
                };
            }
            return result;
        }
        #endregion
        #region Private Methods
        protected override string GetFileName(ColorIndexes color, int value)
        {
            string result = string.Empty;
            Tile t = new(color, value);
            if (t.Source != null)
                result = t.Source.ToString()!;
            return result;
        }
        #endregion
    }
}