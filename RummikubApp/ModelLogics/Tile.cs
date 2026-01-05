using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    public partial class Tile : TileModel
    {
        public Tile(Colors color, int number) : base(color, number)
        {
            Color = color;
            Number = number;
            IsJoker = false;
        }
        public Tile() : base()
        {
            IsJoker = true;
            Number = 0;
            Source = Strings.Joker;
            Aspect = Aspect.AspectFit;
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Start, false);
            WidthRequest = 40;
        }
        public Tile(bool isEmptyTile) : base()
        {
            IsEmptyTile = isEmptyTile;
            IsJoker = false;
            Number = 0;
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
        public override ImageSource? GetSourceFor(Colors color, int number) 
        {
            if (number <= 0) return null;
            string file = GetFileName(color, number);
            return ImageSource.FromFile(file);
        }
        protected override string GetFileName(Colors color, int number) 
        {
            Tile t = new(color, number);
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
            Tile t2 = new((Colors)data.Color, data.Number)
            {
                IsEmptyTile = false
            };
            return t2;
        }
    }
}
