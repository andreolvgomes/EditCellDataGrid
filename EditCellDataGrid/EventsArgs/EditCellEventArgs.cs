using System;
using System.Windows.Controls;

namespace EditCellDataGrid.EventsArgs
{
    public class EditCellEventArgs : EventArgs
    {
        public DataGridRow Row { get; set; }
        public DataGridCell Cell { get; set; }

        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }

    public class ValidateEventArgs : EventArgs
    {
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
