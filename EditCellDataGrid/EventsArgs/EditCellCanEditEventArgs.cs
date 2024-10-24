using System;
using System.Windows.Controls;

namespace EditCellDataGrid.EventsArgs
{
    public class EditCellCanEditEventArgs : EventArgs
    {
        public object Item { get; set; }
        public int SelectedIndex { get; set; }
        public DataGridTextColumn Column { get; set; }
        public string FieldName { get; set; }

        public DataGridRow Row { get; set; }
        public DataGridCell Cell { get; set; }
        public string Id { get; internal set; }
    }
}