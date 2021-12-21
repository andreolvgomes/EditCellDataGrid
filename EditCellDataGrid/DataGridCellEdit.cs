using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EditCellDataGrid
{
    public class DataGridCellEdit<T> where T : class, new()
    {
        private bool _beginEdit = false;

        public void BeginEdit(DataGrid dataGrid, bool defineCellStyle = true)
        {
            if (_beginEdit == true)
                throw new Exception("Execute BeginEdit just once");

            if (defineCellStyle)
                DefineCellStyle(dataGrid);
            dataGrid.BeginningEdit += new EventHandler<DataGridBeginningEditEventArgs>(OnBeginningEdit);
            _beginEdit = true;
        }

        private void OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;

            var dataGrid = sender as DataGrid;
            var selectedItem = dataGrid.SelectedItem;

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
                if (OnDefineNewValue(e.Column as DataGridTextColumn, result) == false)
                {
                    textBlock.Text = result.Value;
                    property.SetValue(selectedItem, Get(property.PropertyType, result.Value));
                }
            }

            // define focus
            selectedCell.Focus();
            selectedRow.Focus();
            selectedRow.IsSelected = true;
        }

        public bool OnDefineNewValue(DataGridTextColumn column, Result result)
        {
            if (column == null) return false;

            Type type = column.GetType();

            var field = type.GetField("DefineNewValue", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) return false;

            var eventDelegate = field.GetValue(column) as MulticastDelegate;
            if (eventDelegate == null)
                return false;

            var events = eventDelegate.GetInvocationList();
            if (events.Length == 0)
                return false;

            foreach (var eventHandler in events)
            {
                eventHandler.Method.Invoke(
                        eventHandler.Target, new object[] { column, result });
            }
            return true;
        }

        /// <summary>
        /// Define color Read for Cell that can edit
        /// </summary>
        /// <param name="dataGrid"></param>
        private void DefineCellStyle(DataGrid dataGrid)
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
        private void DefinePosition(DataGridCell selectedCell, DataGridRow selectedRow, Window view)
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
        private object Get(Type type, string str)
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
        private PropertyInfo GetProperty(DataGrid dataGrid, DataGridBeginningEditEventArgs e)
        {
            var binding = ((System.Windows.Controls.DataGridBoundColumn)e.Column).Binding;
            if (binding == null)
                throw new Exception($"'{e.Column.Header}'... binding is null, not allowed.. define IsReadOnly=True");

            var path = ((System.Windows.Data.Binding)binding).Path.Path;
            var property = dataGrid.SelectedItem.GetType().GetProperty(path);
            return property;
        }
    }
}