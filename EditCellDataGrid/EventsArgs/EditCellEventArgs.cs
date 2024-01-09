using System;
using System.Windows.Controls;

namespace EditCellDataGrid.EventsArgs
{
    public class EditCellEventArgs : EventArgs
    {
        public object Item { get; set; }
        public int SelectedIndex { get; set; }
        public DataGridTextColumn Column { get; set; }
        public string FieldName { get; set; }

        public DataGridRow Row { get; set; }
        public DataGridCell Cell { get; set; }

        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Id { get; internal set; }
    }
}