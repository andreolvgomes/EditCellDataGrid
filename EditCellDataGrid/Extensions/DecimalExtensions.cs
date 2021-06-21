using System.Windows.Controls;

namespace System
{
    public static class DecimalExtensions
    {
        public static bool IsDecimal(this string value)
        {
            decimal result = 0;
            var res = decimal.TryParse(value, out result);
            return res;
        }
    }
}