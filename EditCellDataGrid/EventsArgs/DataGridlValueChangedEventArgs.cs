using System;
using System.Windows.Controls;

namespace EditCellDataGrid.EventsArgs
{
    public class DataGridlValueChangedEventArgs<T> : EventArgs where T : class, new()
    {
        public T Entity { get; set; }
        public DataGridTextColumn Column { get; set; }
        public DataGridRow Row { get; set; }
        public DataGridCell Cell { get; set; }
        public string FieldName { get; set; }

        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}