using EditCellDataGrid;
using System.Windows.Input;

namespace System.Windows.Controls
{
    public static class DataGridExtensions
    {
        public static void MangerEdit<T>(this DataGrid dataGrid) where T : class, new()
        {
            new DataGridCellEdit<T>().BeginEdit(dataGrid);
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

        public static void FocusDataGridCell(this DataGrid dataGrid, int column)
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
    }
}