namespace WebUIFinal.Core
{
    public class Constants
    {
        public static string PagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        public static IEnumerable<int> PageSizeOptions = [5, 10, 20, 30, 100, 200];
        public static string ImagePattern = @"\.(jpg|png)$";
        public static string ValidateChar = "^[^:]*$";//chuoi regex loai tru dau ':'.
    }
}
