namespace RummikubApp.Models
{
    public class TileModel : ImageButton
    {
        private static readonly string[,] tilesImage = {
            { Strings.OneOrange, Strings.OneOrange, Strings.TwoOrange, Strings.TwoOrange, Strings.ThreeOrange, Strings.ThreeOrange, Strings.FourOrange, Strings.FourOrange, Strings.FiveOrange, Strings.FiveOrange, Strings.SixOrange, Strings.SixOrange, Strings.SevenOrange, Strings.SevenOrange, Strings.EightOrange, Strings.EightOrange, Strings.NineOrange, Strings.NineOrange, Strings.TenOrange, Strings.TenOrange, Strings.ElevenOrange, Strings.ElevenOrange, Strings.TwelveOrange, Strings.TwelveOrange, Strings.ThirteenOrange, Strings.ThirteenOrange },
            { Strings.OneRed, Strings.OneRed, Strings.TwoRed, Strings.TwoRed, Strings.ThreeRed, Strings.ThreeRed, Strings.FourRed, Strings.FourRed, Strings.FiveRed, Strings.FiveRed, Strings.SixRed, Strings.SixRed, Strings.SevenRed, Strings.SevenRed, Strings.EightRed, Strings.EightRed, Strings.NineRed, Strings.NineRed, Strings.TenRed, Strings.TenRed, Strings.ElevenRed, Strings.ElevenRed, Strings.TwelveRed, Strings.TwelveRed, Strings.ThirteenRed, Strings.ThirteenRed },
            { Strings.OneBlue, Strings.OneBlue, Strings.TwoBlue, Strings.TwoBlue, Strings.ThreeBlue, Strings.ThreeBlue, Strings.FourBlue, Strings.FourBlue, Strings.FiveBlue, Strings.FiveBlue, Strings.SixBlue, Strings.SixBlue, Strings.SevenBlue, Strings.SevenBlue, Strings.EightBlue, Strings.EightBlue, Strings.NineBlue, Strings.NineBlue, Strings.TenBlue, Strings.TenBlue, Strings.ElevenBlue, Strings.ElevenBlue, Strings.TwelveBlue, Strings.TwelveBlue, Strings.ThirteenBlue, Strings.ThirteenBlue },
            { Strings.OneGreen, Strings.OneGreen, Strings.TwoGreen, Strings.TwoGreen, Strings.ThreeGreen, Strings.ThreeGreen, Strings.FourGreen, Strings.FourGreen, Strings.FiveGreen, Strings.FiveGreen, Strings.SixGreen, Strings.SixGreen, Strings.SevenGreen, Strings.SevenGreen, Strings.EightGreen, Strings.EightGreen, Strings.NineGreen, Strings.NineGreen, Strings.TenGreen, Strings.TenGreen, Strings.ElevenGreen, Strings.ElevenGreen, Strings.TwelveGreen, Strings.TwelveGreen, Strings.ThirteenGreen, Strings.ThirteenGreen }
        };
        public enum Colors
        {
            Red,
            Blue,
            Yellow,
            Black
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
        public bool IsEmpty => Number == 0;
        public TileModel(Colors color, int number)
        {
            Color = color;
            Number = number;
            if (number > 0)
                Source = tilesImage[(int)color, number - 1];
            Aspect = Aspect.AspectFit;
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Start, false);
            WidthRequest = 50;
        }
    }
}
 