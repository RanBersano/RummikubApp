namespace RummikubApp.Models
{
    public class TileData
    {
        public int Color { get; set; }   // (int)TileModel.Colors
        public int Number { get; set; }  // 1–13, או 0 לג'וקר
        public bool IsJoker { get; set; }
    }
}
