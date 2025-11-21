namespace RummikubApp.Models
{
    public class TileModel
    {
        public enum Colors
        {
            Red,
            Blue,
            Yellow,
            Black
        }
        public Colors Color { get; set; }
        public int Number { get; set; }
    }
}
 