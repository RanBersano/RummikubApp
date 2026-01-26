using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    public partial class Tile : TileModel
    {
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
        public override Tile CreateEmptyTile()
        {
            Tile empty = new(true);
            return empty;
        }
        public override ImageSource? GetSourceFor(ColorIndexes color, int value) 
        {
            if (value <= 0) return null;
            string file = GetFileName(color, value);
            return ImageSource.FromFile(file);
        }
        protected override string GetFileName(ColorIndexes color, int value) 
        {
            Tile t = new(color, value);
            if (t.Source == null) return string.Empty;
            return t.Source.ToString()!;
        }
        public override Tile FromTileData(TileData data)
        {
            if (data == null)
                return CreateEmptyTile();
            if (data.IsEmptyTile)
                return CreateEmptyTile();
            if (data.IsJoker)
                return new Tile();
            Tile t2 = new((ColorIndexes)data.ColorIndex, data.Value)
            {
                IsEmptyTile = false
            };
            return t2;
        }
    }
}
