namespace RummikubApp.Models
{
    public class GameSize
    {
        #region Properties
        public int Size { get; set; }
        public string DisplayName => $"{Size} {Strings.Players}";
        #endregion
        #region Constructors
        public GameSize(int size)
        {
            Size = size;
        }
        public GameSize()
        {
            Size = 2;
        }
        #endregion
    }
}
