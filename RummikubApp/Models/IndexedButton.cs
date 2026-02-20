namespace RummikubApp.Models
{
    public partial class IndexedButton : Button
    {
        #region Properties
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        #endregion
        #region Constructor
        public IndexedButton(int rowIndex, int columnIndex)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex + 1;
            HeightRequest = 50;
            WidthRequest = 40;
        }
        #endregion
    }
}
