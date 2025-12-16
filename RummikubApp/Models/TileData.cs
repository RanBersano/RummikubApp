using Plugin.CloudFirestore.Attributes;

namespace RummikubApp.Models
{
    public class TileData
    {
        public int Color { get; set; }      // (int)TileModel.Colors
        public int Number { get; set; }     // 1–13 או 0
        public bool IsJoker { get; set; }
        public bool IsEmptySlot { get; set; }

        // מצב UI בלבד - לא לשמור בפיירסטור
        [Ignored]
        public bool IsSelected { get; set; }
    }
}