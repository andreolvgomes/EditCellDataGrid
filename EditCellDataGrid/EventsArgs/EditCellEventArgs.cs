using System;

namespace EditCellDataGrid.EventsArgs
{
    public class EditCellEventArgs : EventArgs
    {
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
