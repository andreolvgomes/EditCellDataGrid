using EditCellDataGrid.EventsArgs;
using EditCellDataGrid.Helpers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using System.Windows;
using System;
using EditCellDataGrid.Extenders;

namespace EditCellDataGrid
{
    public class DataGridCellEdit<T> where T : class, new()
    {
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
                DefineCellStyle(dataGrid);

            dataGrid.BeginningEdit += new EventHandler<DataGridBeginningEditEventArgs>(OnBeginningEdit);
            _beginEdit = true;
        }

        private void OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;

            var selectedItem = _datagrid.SelectedItem;

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

            if (CheckInput(property.PropertyType, value, typeInput) == false)
            {
                _datagrid.CancelEdit();
                _datagrid.CommitEdit();
            }
            else
            {
                var view = new EditCell(Window.GetWindow(_datagrid), textBlock.Text, value, typeInput, e.Column, property.PropertyType);
                view.DefineStyleTextBox(_datagrid, rowSelected);
                
                view.Field.PreviewKeyDown += FieldPreviewKeyDown;
                view.lblRotulo.Text = e.Column.Header.ToString();

                DefinePosition(cellSelected, rowSelected, view);

                var result = view.Input();
                if (result.Success)
                {
                    if (result.Changes)
                    {
                        var eventArgs = new EditCellEventArgs()
                        {
                            Row = rowSelected,
                            Cell = cellSelected,
                            NewValue = result.NewValue,
                            OldValue = result.OldValue
                        };

                        property.SetValue(selectedItem, Convert.ChangeType(result.NewValue, property.PropertyType));

                        OnNewValueConfirmed(e.Column as DataGridTextColumn, eventArgs);
                    }

                    if (result.PressedEnter)
                        _datagrid.MoveNextRow();
                }
            }
        }

        private bool CheckInput(Type type, string value, TypeInput typeInput)
        {
            if (typeInput != TypeInput.KeyboardDevice)
                return true;

            // se é mascarada, então só pode números
            if (_column as TextColumnEditMask != null)
                return long.TryParse(value, out _);

            if (CheckTypeWithInputText.CheckStartInputNumberValid(type, value) == false)
                return false;
            return true;
        }

        private void FieldPreviewKeyDown(object sender, KeyEventArgs e)
        {
            OnPreviewKeyDown(sender, e);
        }

        public bool OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_column == null) return false;

            Type type = _column.GetType();

            var field = type.GetField("EventPreviewKeyDown", BindingFlags.NonPublic | BindingFlags.Instance);
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
                        eventHandler.Target, new object[] { sender, e });
            }
            return true;
        }

        public bool OnNewValueConfirmed(DataGridTextColumn column, EditCellEventArgs eventArgs)
        {
            if (column == null) return false;

            Type type = column.GetType();

            var field = type.GetField("EventNewValueConfirmed", BindingFlags.NonPublic | BindingFlags.Instance);
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
        /// Define color Read for Cell that can edit
        /// </summary>
        /// <param name="dataGrid"></param>
        private void DefineCellStyle(DataGrid dataGrid)
        {
            var multiTrigger = new MultiTrigger();

            // rules
            multiTrigger.Conditions.Add(new Condition(DataGridCell.IsKeyboardFocusWithinProperty, true));
            multiTrigger.Conditions.Add(new Condition(DataGridCell.IsReadOnlyProperty, false));

            multiTrigger.Setters.Add(new Setter(DataGridCell.BorderThicknessProperty, new Thickness(1)));
            multiTrigger.Setters.Add(new Setter(DataGridCell.FocusVisualStyleProperty, null));
            multiTrigger.Setters.Add(new Setter(DataGridCell.BorderBrushProperty, new SolidColorBrush(Colors.Teal)));
            multiTrigger.Setters.Add(new Setter(DataGridCell.ForegroundProperty, new SolidColorBrush(Colors.Black)));
            multiTrigger.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.LightGray)));

            var style = new Style();
            style.Triggers.Add(multiTrigger);

            dataGrid.CellStyle = style;
        }

        /// <summary>
        /// Define position of the Window Edit
        /// </summary>
        /// <param name="cellSelected"></param>
        /// <param name="rowSelected"></param>
        /// <param name="view"></param>
        private void DefinePosition(DataGridCell cellSelected, DataGridRow rowSelected, Window view)
        {
            view.MinWidth = cellSelected.ActualWidth;// + 20;

            var source = PresentationSource.FromVisual(cellSelected);
            var dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
            var dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;

            var scalingFactorX = dpiX / 96.0;
            var scalingFactorY = dpiY / 96.0;

            var coordinate = cellSelected.PointToScreen(new Point(0, 0));

            view.Left = coordinate.X / scalingFactorX;
            view.Top = (coordinate.Y / scalingFactorY) - 1;// - 19;//-27; move um pouco pra cima
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