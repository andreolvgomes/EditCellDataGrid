using EditCellDataGrid.Extensions;
using System.Windows.Input;
using EditCellDataGrid;

namespace System.Windows.Controls
{
    public static class DataGridExtensions
    {
        public static DataGridCellEdit<T> MangerEdit<T>(this DataGrid dataGrid) where T : class, new()
        {
            var dataGridCellEdit = new DataGridCellEdit<T>();
            dataGridCellEdit.BeginEdit(dataGrid);
            return dataGridCellEdit;
        }

        public static void MoveNextRow(this DataGrid dataGrid)
        {
            dataGrid.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (dataGrid.Items.Count - 1 > dataGrid.SelectedIndex)
                {
                    var displayIndex = dataGrid.CurrentCell.Column.DisplayIndex;
                    dataGrid.SelectedIndex += 1;

                    var row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    if (row != null)
                    {
                        row.Focus();
                        row.IsSelected = true;
                        row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        FocusDataGridCell(dataGrid, displayIndex);
                    }
                }
            }));
        }

        private static void FocusDataGridCell(this DataGrid dataGrid, int column)
        {
            if (dataGrid.IsKeyboardFocusWithin && dataGrid.SelectedItem != null)
            {
                // make sure everything is up to date
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);

                var cellcontent = dataGrid.Columns[column].GetCellContent(dataGrid.SelectedItem);
                var cell = cellcontent?.Parent as DataGridCell;
                if (cell != null)
                    cell.Focus();
            }
        }

        public static void SetStyle(this DataGrid datagrid, Style style)
        {
            datagrid.StyleCell(style);
        }
    }
}