namespace RummikubApp.Models
{
    public class IndexedButton : Button
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public IndexedButton(int rowIndex, int columnIndex)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex + 1;
            HeightRequest = 50;
            WidthRequest = 40;
        }
    }
}
