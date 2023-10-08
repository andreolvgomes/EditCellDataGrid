using System;

namespace EditCellDataGrid.EventsArgs
{
    public class ValidateEventArgs : EventArgs
    {
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
