﻿using EditCellDataGrid.EventsArgs;
using EditCellDataGrid.Extensions;
using EditCellDataGrid.Delegates;
using System.Windows.Controls;
using System.Reflection;
using System.Windows;
using System;

namespace EditCellDataGrid
{
    public class DataGridCellEdit<T> where T : class, new()
    {
        public event DataGridlValueChangedEventHanddler<T> EventDataGridValueChanged;

        private object modelSelected;
        private string fieldName;

        private PropertyInfo property;
        private DataGridRow rowSelected;
        private DataGridCell cellSelected;
        private EditCell view;

        private bool closedOwner = false;
        private Window owner = null;
        private DataGrid _datagrid = null;
        private bool _beginEdit = false;
        private DataGridTextColumn column;

        public void BeginEdit(DataGrid dataGrid, bool defineCellStyle = true)
        {
            _datagrid = dataGrid;
            owner = Window.GetWindow(_datagrid);
            owner.Closed += OwnerClosed;

            if (_beginEdit == true)
                throw new Exception("Execute BeginEdit just once");

            if (dataGrid.IsReadOnly)
                throw new Exception("DataGrid IsReadOnly=true, define IsReadOnly=false");

            if (defineCellStyle)
                dataGrid.CreateStyleCell();

            dataGrid.BeginningEdit += new EventHandler<DataGridBeginningEditEventArgs>(OnBeginningEdit);
            _beginEdit = true;            
        }

        private void OwnerClosed(object sender, EventArgs e)
        {
            closedOwner = true;
            if (view != null)
            {
                view.Close();
                view = null;
            }
        }

        private void OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;

            if (_datagrid.CurrentColumn as DataGridTemplateColumn != null)
                return;

            modelSelected = _datagrid.SelectedItem;

            rowSelected = _datagrid.GetSelectedRow();
            cellSelected = _datagrid.GetCell(rowSelected, _datagrid.CurrentColumn.DisplayIndex);
            property = GetProperty(_datagrid, e);
            column = e.Column as DataGridTextColumn;

            var textBlock = (cellSelected.Content as TextBlock);
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

            if (CheckInputWithKeyboardDeviceIsValid(property.PropertyType, value, typeInput) == false)
            {
                _datagrid.CancelEdit();
                _datagrid.CommitEdit();
            }
            else
            {
                if (view != null)
                    view.Close();

                view = new EditCell(owner, textBlock.Text, value, typeInput, e.Column, property.PropertyType);
                view.SettingsField(_datagrid, rowSelected, e.Column.Header.ToString());

                view.MinWidth = cellSelected.ActualWidth;
                view.DefinePosition(cellSelected);
                view.Closed += InputClosed;
                view.Show();
            }
        }

        private void InputClosed(object sender, EventArgs e)
        {
            if (closedOwner == false)
            {
                view = null;
                var input = sender as EditCell;
                var result = input.Get();
                if (result.Success)
                {
                    if (result.Changes)
                    {
                        property.SetValue(modelSelected, Convert.ChangeType(result.NewValue, property.PropertyType));

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
                    Entity = (T)modelSelected,
                    Column = column,
                    Row = dataGridRow,
                    Cell = dataGridCell,
                    FieldName = fieldName,
                    OldValue = result.OldValue,
                    NewValue = result.NewValue
                });
            }
        }

        private bool CheckInputWithKeyboardDeviceIsValid(Type type, string value, TypeInput typeInput)
        {
            if (typeInput != TypeInput.KeyboardDevice)
                return true;

            if (column.CheckColInputIsNumber(type))
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

            if (column == null) return false;

            Type type = column.GetType();

            var field = type.GetField("EventCellValueChanged", BindingFlags.NonPublic | BindingFlags.Instance);
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
                        eventHandler.Target, new object[] { column, eventArgs });
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

            fieldName = ((System.Windows.Data.Binding)binding).Path.Path;
            var property = dataGrid.SelectedItem.GetType().GetProperty(fieldName);
            return property;
        }
    }
}