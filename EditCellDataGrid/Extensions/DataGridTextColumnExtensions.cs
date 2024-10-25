using EditCellDataGrid.Extenders;
using System.Windows.Controls;
using System;

namespace EditCellDataGrid.Extensions
{
    public static class DataGridTextColumnExtensions
    {
        public static bool CheckColInputIsNumber(this DataGridTextColumn col, Type type)
        {
            if (col as TextColumnEditMask != null)
                return true;
            if (col as TextColumnEditDate != null)
                return true;
            if (col as TextColumnEditInteger != null)
                return true;
            if (col as TextColumnEditDecimal != null)
                return true;

            if (type == typeof(DateTime))
                return true;
            if (type == typeof(int) || type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64))
                return true;
            if (type == typeof(decimal))
                return true;

            return false;
        }

        public static bool IsColDecimal(this DataGridTextColumn col, Type type)
        {
            if (col as TextColumnEditDecimal != null)
                return true;
            return false;
        }
    }
}