using System;

namespace EditCellDataGrid.Extensions
{
    public static class StringExtensions
    {
        public static bool CheckNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string SubstringBest(this string str, int startIndex, int length)
        {
            if (string.IsNullOrEmpty(str)) return "";

            if (startIndex < 0)
                return str.SubstringBest(0, length);

            if (startIndex > str.Length)
                return "";

            if (length < 0)
                throw new Exception("length não pode ser menor que zero");

            if (length == 0)
                return "";

            if (length > str.Length)
                return str.Substring(startIndex, str.Length - startIndex);

            if (startIndex + length > str.Length)
                return str.Substring(startIndex, str.Length - startIndex);

            return str.Substring(startIndex, length);
        }
    }
}