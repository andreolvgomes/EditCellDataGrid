using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace EditCellDataGrid.Extensions
{
    public static class StyleCellDataGrid
    {
        /// <summary>
        /// Define color Read for Cell that can edit
        /// </summary>
        /// <param name="datagrid"></param>
        public static void CreateStyleCell(this DataGrid datagrid)
        {
            var multiTrigger = new MultiTrigger();

            // rules
            multiTrigger.Conditions.Add(new Condition(DataGridCell.IsKeyboardFocusWithinProperty, true));
            multiTrigger.Conditions.Add(new Condition(DataGridCell.IsReadOnlyProperty, false));

            //multiTrigger.Setters.Add(new Setter(DataGridCell.BorderThicknessProperty, new Thickness(1)));
            //multiTrigger.Setters.Add(new Setter(DataGridCell.FocusVisualStyleProperty, null));
            //multiTrigger.Setters.Add(new Setter(DataGridCell.BorderBrushProperty, new SolidColorBrush(Colors.Teal)));
            //multiTrigger.Setters.Add(new Setter(DataGridCell.ForegroundProperty, new SolidColorBrush(Colors.Black)));
            //multiTrigger.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.LightGray)));

            multiTrigger.Setters.Add(new Setter(DataGridCell.BorderThicknessProperty, new Thickness(2)));
            multiTrigger.Setters.Add(new Setter(DataGridCell.FocusVisualStyleProperty, null));
            multiTrigger.Setters.Add(new Setter(DataGridCell.BorderBrushProperty, new SolidColorBrush(Colors.Teal)));
            multiTrigger.Setters.Add(new Setter(DataGridCell.ForegroundProperty, new SolidColorBrush(Colors.Black)));
            multiTrigger.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.White)));

            datagrid.StyleCellMultiTrigger(multiTrigger);
        }

        public static void StyleCell(this DataGrid datagrid, Style style)
        {
            datagrid.CellStyle = style;
        }

        public static void StyleCellMultiTrigger(this DataGrid datagrid, MultiTrigger multiTrigger)
        {
            if (datagrid.CellStyle == null)
            {
                var style = new Style();
                datagrid.CellStyle = style;
            }
            datagrid.CellStyle.Triggers.Add(multiTrigger);
        }
    }
}