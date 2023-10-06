using System;

namespace EditCellDataGrid.Helpers
{
    public class CheckTypeWithInputText
    {
        public static bool CheckInputIsValid(Type type, string input)
        {
            if (type == typeof(DateTime))
                return DateTime.TryParse(input, out _);
            if (type == typeof(int) || type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64))
                return long.TryParse(input, out _);
            if (type == typeof(decimal))
                return decimal.TryParse(input, out _);
            return true;
        }

        public static bool CheckStartInputNumberValid(Type type, string input)
        {
            if (type == typeof(DateTime))// no início da entrada confere se tá entrando com número
                return long.TryParse(input, out _);
            if (type == typeof(int) || type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64))
                return long.TryParse(input, out _);
            if (type == typeof(decimal))
                return decimal.TryParse(input, out _);
            return true;
        }
    }
}