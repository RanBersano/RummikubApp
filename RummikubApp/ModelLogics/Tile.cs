using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    public class Tile : TileModel
    {
        public Tile(Colors color, int number)
        {
            Color = color;
            Number = number;
        }

        public override string ToString()
        {
            return $"{Color} {Number}";
        }
    }
}
