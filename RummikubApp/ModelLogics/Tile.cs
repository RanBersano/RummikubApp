using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    public class Tile : TileModel
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
        public Tile(bool isEmptySlot) : base()
        {
            IsEmptySlot = isEmptySlot;
            IsJoker = false;
            Number = 0;
            Source = null;
            Aspect = Aspect.AspectFit;
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Start, false);
            WidthRequest = 40;
        }
        public static Tile CreateEmptySlot()
        {
            Tile empty = new Tile(true);
            return empty;
        }


    }
}
