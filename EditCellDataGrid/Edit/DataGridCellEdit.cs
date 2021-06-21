using EditCellDataGrid.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EditCellDataGrid.Edit
{
    public class DataGridCellEdit
    {
        public static void BeginEdit(DataGrid dataGrid, bool defineCellStyle = true)
        {
            if (defineCellStyle)
                DefineCellStyle(dataGrid);
            dataGrid.BeginningEdit += new EventHandler<DataGridBeginningEditEventArgs>(BeginningEdit);
        }

        private static void BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;

            var dataGrid = sender as DataGrid;

            var selectedRow = dataGrid.GetSelectedRow();
            var selectedCell = dataGrid.GetCell(selectedRow, dataGrid.CurrentColumn.DisplayIndex);

            var textBlock = (selectedCell.Content as TextBlock);
            var value = textBlock.Text;
            var typeInput = TypeInput.F2Native;

            // F2(nativo)
            if (e.EditingEventArgs == null)
            {
                value = textBlock.Text;
                typeInput = TypeInput.F2Native;
            }
            // input pelo teclado, digitou algum caracter
            else if (((System.Windows.Input.InputEventArgs)e.EditingEventArgs).Device.ToString().Equals("System.Windows.Input.Win32KeyboardDevice"))
            {
                var editingEventArgs = (e.EditingEventArgs as System.Windows.Input.TextCompositionEventArgs);
                value = editingEventArgs.Text;
                typeInput = TypeInput.KeyboardDevice;
            }
            // clique para editar
            else if (((System.Windows.Input.InputEventArgs)e.EditingEventArgs).Device.ToString().Equals("System.Windows.Input.Win32MouseDevice"))
            {
                value = textBlock.Text;
                typeInput = TypeInput.MouseDevice;
            }

            var property = GetProperty(dataGrid, e);
            var view = new EditCell(Window.GetWindow(dataGrid), value, typeInput, e.Column as DataGridTextColumn, property.PropertyType);
            view.lblRotulo.Text = e.Column.Header.ToString();

            DefinePosition(selectedCell, selectedRow, view);

            var result = view.Get();
            if (result.Success)
            {
                textBlock.Text = result.Value;

                var selecteObj = dataGrid.SelectedItem;
                property.SetValue(selecteObj, Get(property.PropertyType, result.Value));
            }
        }

        /// <summary>
        /// Define color Read for Cell that can edit
        /// </summary>
        /// <param name="dataGrid"></param>
        private static void DefineCellStyle(DataGrid dataGrid)
        {
            MultiTrigger multiTrigger = new MultiTrigger();

            // rules
            multiTrigger.Conditions.Add(new Condition(DataGridCell.IsKeyboardFocusWithinProperty, true));
            multiTrigger.Conditions.Add(new Condition(DataGridCell.IsReadOnlyProperty, false));

            multiTrigger.Setters.Add(new Setter(DataGridCell.BorderThicknessProperty, new Thickness(1)));
            multiTrigger.Setters.Add(new Setter(DataGridCell.FocusVisualStyleProperty, null));
            multiTrigger.Setters.Add(new Setter(DataGridCell.BorderBrushProperty, new SolidColorBrush(Colors.Red)));
            multiTrigger.Setters.Add(new Setter(DataGridCell.ForegroundProperty, new SolidColorBrush(Colors.Black)));
            multiTrigger.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.White)));

            Style style = new Style();
            style.Triggers.Add(multiTrigger);

            dataGrid.CellStyle = style;
        }

        /// <summary>
        /// Define position of the Window Edit
        /// </summary>
        /// <param name="selectedCell"></param>
        /// <param name="selectedRow"></param>
        /// <param name="view"></param>
        private static void DefinePosition(DataGridCell selectedCell, DataGridRow selectedRow, Window view)
        {
            var screenCoordinates = selectedCell.PointToScreen(new Point(0, 0));
            view.Width = selectedCell.ActualWidth + 20;
            view.Left = screenCoordinates.X;

            var pointRow = selectedRow.PointToScreen(new Point(0, 0));
            view.Top = pointRow.Y;
        }

        /// <summary>
        /// Convert value
        /// </summary>
        /// <param name="type"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private static object Get(Type type, string str)
        {
            System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(type);
            return converter.ConvertFrom(str);
        }

        /// <summary>
        /// Get property from datagrid column
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private static PropertyInfo GetProperty(DataGrid dataGrid, DataGridBeginningEditEventArgs e)
        {
            var binding = ((System.Windows.Controls.DataGridBoundColumn)e.Column).Binding;
            if (binding == null)
                throw new Exception($"'{e.Column.Header}'... binding is null, not allowed.. define IsReadOnly=True");

            var path = ((System.Windows.Data.Binding)binding).Path.Path;
            var selecteObj = dataGrid.SelectedItem as Produto;
            var property = selecteObj.GetType().GetProperty(path);
            return property;
        }
    }
}