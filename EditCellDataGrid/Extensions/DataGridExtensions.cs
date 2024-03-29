﻿using EditCellDataGrid.Extensions;
using EditCellDataGrid.Extenders;
using System.Windows.Input;
using EditCellDataGrid;

namespace System.Windows.Controls
{
    public static class DataGridExtensions
    {
        public static DataGridCellEdit<T> ManagerEdit<T>(this DataGrid dataGrid, bool defineCellStyle = true, CellEditSettings settings = null) where T : class, new()
        {
            var dataGridCellEdit = new DataGridCellEdit<T>();
            dataGridCellEdit.BeginEdit(dataGrid, defineCellStyle: defineCellStyle, settings: settings);
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

        public static DataGridColumn ColById(this DataGrid datagrid, string Id)
        {
            if (datagrid.Columns.Count == 0) return null;

            for (int i = 0; i < datagrid.Columns.Count; i++)
            {
                var dataGridTemplateColumnExtender = datagrid.Columns[i] as TextColumnEdit;
                if (dataGridTemplateColumnExtender != null)
                {
                    if (dataGridTemplateColumnExtender.Id == null) continue;
                    if (dataGridTemplateColumnExtender.Id.ToUpper().Equals(Id.ToUpper()))
                        return dataGridTemplateColumnExtender;
                }
            }
            return null;
        }
    }
}