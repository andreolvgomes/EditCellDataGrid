using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EditCellDataGrid.Extenders
{
    public class EditCellEventArgs : EventArgs
    {
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }

    public delegate bool EditCellValidationEventHandler(object sender, EditCellEventArgs e);

    public class DataGridTextColumnEditCellValidation : DataGridTextColumn
    {
        public event EditCellValidationEventHandler Validation;
    }
}