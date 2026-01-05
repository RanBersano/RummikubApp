using RummikubApp.ModelLogics;

namespace RummikubApp.Models
{
    public abstract class TileModel : ImageButton
    {
        private static readonly string[,] tilesImage ={
            {Strings.OneOrange, Strings.TwoOrange, Strings.ThreeOrange, Strings.FourOrange, Strings.FiveOrange, Strings.SixOrange, Strings.SevenOrange, Strings.EightOrange, Strings.NineOrange, Strings.TenOrange, Strings.ElevenOrange, Strings.TwelveOrange, Strings.ThirteenOrange},
            {Strings.OneRed, Strings.TwoRed, Strings.ThreeRed, Strings.FourRed, Strings.FiveRed, Strings.SixRed, Strings.SevenRed, Strings.EightRed, Strings.NineRed, Strings.TenRed, Strings.ElevenRed, Strings.TwelveRed, Strings.ThirteenRed},
            {Strings.OneBlue, Strings.TwoBlue, Strings.ThreeBlue, Strings.FourBlue, Strings.FiveBlue, Strings.SixBlue, Strings.SevenBlue, Strings.EightBlue, Strings.NineBlue, Strings.TenBlue, Strings.ElevenBlue, Strings.TwelveBlue, Strings.ThirteenBlue},
            {Strings.OneGreen, Strings.TwoGreen, Strings.ThreeGreen, Strings.FourGreen, Strings.FiveGreen, Strings.SixGreen, Strings.SevenGreen, Strings.EightGreen, Strings.NineGreen, Strings.TenGreen, Strings.ElevenGreen, Strings.TwelveGreen, Strings.ThirteenGreen}};
        public enum Colors
        {
            Orange,
            Red,
            Blue,
            Green
        }
        public static int TilesInColor
        {
            get
            {
                return tilesImage.GetLength(1);
            }
        }
        public Colors Color { get; set; }
        public int Number { get; set; }
        public bool IsJoker { get; set; }
        public bool IsSelected { get; set; }
        public int Index { get; set; }
        public bool IsEmptyTile { get; set; }
        public abstract Tile CreateEmptyTile();
        public abstract ImageSource? GetSourceFor(Colors color, int number);
        protected abstract string GetFileName(Colors color, int number);
        public abstract Tile FromTileData(TileData data);
        public TileModel(Colors color, int number)
        {
            Color = color;
            Number = number;
            if (number > 0)
                Source = tilesImage[(int)color, number - 1];
            Aspect = Aspect.AspectFit;
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Start, false);
            WidthRequest = 40;
        }
        public TileModel()
        {
        }
    }
}
 