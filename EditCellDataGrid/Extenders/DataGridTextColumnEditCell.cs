using EditCellDataGrid.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace EditCellDataGrid.Extenders
{
    public class DataGridTextColumnEditCell : DataGridTextColumn
    {
        public event EditCellValidationEventHandler Validation;
        public event DefineNewValueEventHandler DefineNewValue;
        public event KeyEventHandler PreviewKeyDown;
    }
}