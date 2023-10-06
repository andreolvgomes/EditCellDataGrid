using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using System.Windows;
using System;
using EditCellDataGrid.Extenders;
using EditCellDataGrid.EventsArgs;

namespace EditCellDataGrid
{
    public class DataGridCellEdit<T> where T : class, new()
    {
        private bool _beginEdit = false;

        public void BeginEdit(DataGrid dataGrid, bool defineCellStyle = true)
        {
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

            var dataGrid = sender as DataGrid;
            var selectedItem = dataGrid.SelectedItem;

            var rowSelected = dataGrid.GetSelectedRow();
            var cellSelected = dataGrid.GetCell(rowSelected, dataGrid.CurrentColumn.DisplayIndex);

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

            var property = GetProperty(dataGrid, e);
            var view = new EditCell(Window.GetWindow(dataGrid), textBlock.Text, value, typeInput, e.Column, property.PropertyType);
            view.DefineStyleTextBox(dataGrid, rowSelected);

            _column = e.Column as DataGridTextColumn;
            view.textbox.PreviewKeyDown += FieldPreviewKeyDown;
            view.lblRotulo.Text = e.Column.Header.ToString();

            DefinePosition(cellSelected, rowSelected, view);

            var result = view.Get();
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

                    if (OnDefineNewValue(e.Column as DataGridTextColumn, eventArgs) == false)
                        property.SetValue(selectedItem, Convert.ChangeType(result.NewValue, property.PropertyType));

                    OnNewValueConfirmed(e.Column as DataGridTextColumn, eventArgs);
                }
                if (result.PressedEnter)
                    dataGrid.MoveNextRow();
            }
        }

        private void FieldPreviewKeyDown(object sender, KeyEventArgs e)
        {
            OnPreviewKeyDown(sender, e);
        }

        public bool OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_column == null) return false;

            Type type = _column.GetType();

            var field = type.GetField("PreviewKeyDown", BindingFlags.NonPublic | BindingFlags.Instance);
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

        private DataGridTextColumn _column;

        public bool OnDefineNewValue(DataGridTextColumn column, EditCellEventArgs eventArgs)
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
                        eventHandler.Target, new object[] { column, eventArgs });
            }
            return true;
        }

        public bool OnNewValueConfirmed(DataGridTextColumn column, EditCellEventArgs eventArgs)
        {
            if (column == null) return false;

            Type type = column.GetType();

            var field = type.GetField("NewValueConfirmed", BindingFlags.NonPublic | BindingFlags.Instance);
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
        /// <param name="cellSelected"></param>
        /// <param name="rowSelected"></param>
        /// <param name="view"></param>
        private void DefinePosition(DataGridCell cellSelected, DataGridRow rowSelected, Window view)
        {
            var scaler = GetWindowsScaling();
            view.MinWidth = cellSelected.ActualWidth;// + 20;

            if (scaler == 100)
            {
                var screenCoordinates = cellSelected.PointToScreen(new Point(0, 0));
                view.Left = screenCoordinates.X;

                var pointRow = rowSelected.PointToScreen(new Point(0, 0));
                view.Top = pointRow.Y - 27;//-27; joga um pouco pra cima
            }
            else
            {
                view.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
        }

        public static int GetWindowsScaling()
        {
            return (int)(100 * System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth);
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