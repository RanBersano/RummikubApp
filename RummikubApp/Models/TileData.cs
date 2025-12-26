using Plugin.CloudFirestore.Attributes;

namespace RummikubApp.Models
{
    public class TileData
    {
        public int Color { get; set; }
        public int Number { get; set; }
        public bool IsJoker { get; set; }
        public bool IsEmptySlot { get; set; }
        public bool IsPresent { get; set; }
    }
}