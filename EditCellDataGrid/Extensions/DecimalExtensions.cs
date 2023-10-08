using System.Windows.Controls;

namespace EditCellDataGrid.Extensions
{
    public static class DecimalExtensions
    {
        public static bool CheckDecimal(this string value)
        {
            decimal result = 0;
            var res = decimal.TryParse(value, out result);
            return res;
        }
    }
}