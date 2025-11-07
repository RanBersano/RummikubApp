namespace RummikubApp.Models
{
    public class GameSize
    {
        public int Size { get; set; }
        public string DisplayName => $"{Size} Players";
        public GameSize(int size)
        {
            Size = size;
        }
        public GameSize()
        {
            Size = 2;
        }
    }
}
