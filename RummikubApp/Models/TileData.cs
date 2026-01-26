using Plugin.CloudFirestore.Attributes;

namespace RummikubApp.Models
{
    public class TileData
    {
        public int ColorIndex { get; set; }
        public int Value { get; set; }
        public bool IsJoker { get; set; }
        public bool IsEmptyTile { get; set; }
        public bool IsPresent { get; set; }
    }
}