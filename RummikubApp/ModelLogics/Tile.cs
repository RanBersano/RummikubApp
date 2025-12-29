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
        public static ImageSource? GetSourceFor(TileModel.Colors color, int number)
        {
            // number 1..13
            if (number <= 0) return null;
            // TileModel בונה Source עם Strings.* שהם שמות קבצים
            // נחזיר ImageSource לפי אותו שם
            string file = GetFileName(color, number);
            return ImageSource.FromFile(file);
        }
        private static string GetFileName(TileModel.Colors color, int number)
        {
            // כאן משתמשים באותו מערך של TileModel (tilesImage)
            // אבל tilesImage הוא private שם, אז נעשה switch פשוט:
            // שים לב: אתה כבר מחזיק Strings.OneOrange וכו'
            // לכן נבנה לפי צבע/מספר בצורה ידנית.

            // הכי פשוט: ליצור Tile זמני ולקחת ממנו Source
            Tile t = new Tile(color, number);
            if (t.Source == null) return string.Empty;
            return t.Source.ToString()!;
        }
        public static Tile FromTileData(TileData data)
        {
            if (data == null)
                return CreateEmptySlot();
            if (data.IsEmptySlot)
                return CreateEmptySlot();
            if (data.IsJoker)
                return new Tile();
            Tile t2 = new Tile((TileModel.Colors)data.Color, data.Number);
            t2.IsEmptySlot = false;
            return t2;
        }
    }
}
