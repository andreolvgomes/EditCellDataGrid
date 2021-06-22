using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditCellDataGrid.EventsArgs
{
    public class EditCellEventArgs : EventArgs
    {
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}
