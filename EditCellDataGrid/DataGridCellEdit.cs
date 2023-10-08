using EditCellDataGrid.EventsArgs;
using EditCellDataGrid.Extensions;
using System.Windows.Controls;
using System.Reflection;
using System.Windows;
using System;
using EditCellDataGrid.Delegates;

namespace EditCellDataGrid
{
    public class DataGridCellEdit<T> where T : class, new()
    {
        public event DataGridlValueChangedEventHanddler<T> EventDataGridValueChanged;
        private object ModelSelected;
        private string FieldName;

        private DataGrid _datagrid = null;
        private bool _beginEdit = false;
        private DataGridTextColumn _column;

        public void BeginEdit(DataGrid dataGrid, bool defineCellStyle = true)
        {
            _datagrid = dataGrid;

            if (_beginEdit == true)
                throw new Exception("Execute BeginEdit just once");

            if (dataGrid.IsReadOnly)
                throw new Exception("DataGrid IsReadOnly=true, define IsReadOnly=false");

            if (defineCellStyle)
                dataGrid.CreateStyleCell();

            dataGrid.BeginningEdit += new EventHandler<DataGridBeginningEditEventArgs>(OnBeginningEdit);
            _beginEdit = true;
        }

        private void OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;

            ModelSelected = _datagrid.SelectedItem;

            var rowSelected = _datagrid.GetSelectedRow();
            var cellSelected = _datagrid.GetCell(rowSelected, _datagrid.CurrentColumn.DisplayIndex);

            var textBlock = (cellSelected.Content as TextBlock);
            var value = textBlock.Text;
            var typeInput = TypeInput.F2Native;
            var property = GetProperty(_datagrid, e);
            _column = e.Column as DataGridTextColumn;

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

            if (CheckInputWithKeyboardDeviceIsValid(property.PropertyType, value, typeInput) == false)
            {
                _datagrid.CancelEdit();
                _datagrid.CommitEdit();
            }
            else
            {
                var view = new EditCell(Window.GetWindow(_datagrid), textBlock.Text, value, typeInput, e.Column, property.PropertyType);
                view.SettingsField(_datagrid, rowSelected, e.Column.Header.ToString());

                view.MinWidth = cellSelected.ActualWidth;
                view.DefinePosition(cellSelected);

                var result = view.Input();
                if (result.Success)
                {
                    if (result.Changes)
                    {
                        property.SetValue(ModelSelected, Convert.ChangeType(result.NewValue, property.PropertyType));

                        OnNewValueConfirmed(rowSelected, cellSelected, result);
                        OnEventDataGridValueChanged(rowSelected, cellSelected, result);
                    }

                    if (result.KeyEnterPressed)
                        _datagrid.MoveNextRow();
                }
            }
        }

        private void OnEventDataGridValueChanged(DataGridRow dataGridRow, DataGridCell dataGridCell, Result result)
        {
            if (EventDataGridValueChanged != null)
            {
                EventDataGridValueChanged(this, new DataGridlValueChangedEventArgs<T>()
                {
                    Entity = (T)ModelSelected,
                    Column = _column,
                    Row = dataGridRow,
                    Cell = dataGridCell,
                    FieldName = FieldName,
                    OldValue = result.OldValue,
                    NewValue = result.NewValue
                });
            }
        }

        private bool CheckInputWithKeyboardDeviceIsValid(Type type, string value, TypeInput typeInput)
        {
            if (typeInput != TypeInput.KeyboardDevice)
                return true;

            if (_column.CheckColInputIsNumber(type))
                return long.TryParse(value, out _);

            return true;
        }

        public bool OnNewValueConfirmed(DataGridRow dataGridRow, DataGridCell dataGridCell, Result result)
        {
            var eventArgs = new EditCellEventArgs()
            {
                Row = dataGridRow,
                Cell = dataGridCell,
                NewValue = result.NewValue,
                OldValue = result.OldValue
            };

            if (_column== null) return false;

            Type type = _column.GetType();

            var field = type.GetField("EventCellValueChanged", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) return false;

            var eventDelegate = field.GetValue(_column) as MulticastDelegate;
            if (eventDelegate == null)
                return false;

            var events = eventDelegate.GetInvocationList();
            if (events.Length == 0)
                return false;

            foreach (var eventHandler in events)
            {
                eventHandler.Method.Invoke(
                        eventHandler.Target, new object[] { _column, eventArgs });
            }
            return true;
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

            FieldName = ((System.Windows.Data.Binding)binding).Path.Path;
            var property = dataGrid.SelectedItem.GetType().GetProperty(FieldName);
            return property;
        }
    }
}