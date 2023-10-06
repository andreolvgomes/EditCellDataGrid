using EditCellDataGrid.EventsArgs;
using EditCellDataGrid.Extenders;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using System.Windows;
using System.Linq;
using System;
using EditCellDataGrid.Helpers;

namespace EditCellDataGrid
{
    public class Result
    {
        public bool PressedEnter { get; set; }
        public bool Success { get; set; }
        public bool Changes { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }
    }

    public enum TypeInput
    {
        F2Native,
        KeyboardDevice,
        MouseDevice
    }

    public partial class EditCell : Window
    {
        public TextBox Field { get; set; }

        private bool pressedEnter = false;
        private bool closed = false;
        private bool validating = false;
        private bool f2pressed = false;
        private bool success = false;

        private readonly DataGridColumn _column;
        private readonly string _oldValue;
        private readonly Type _propertyType = null;

        public EditCell(Window owner, string oldValue, string value, TypeInput typeInput, DataGridColumn column, Type propertyType)
        {
            InitializeComponent();

            Owner = owner;
            _column = column;
            _propertyType = propertyType;
            _oldValue = oldValue;

            CrateTextBox();
            Field.Text = value.ToUpper();
            Field.Focus();

            if (typeInput == TypeInput.KeyboardDevice)
                Field.SelectionStart = value.Length;
            else
                Field.DefineFocusSelectAll();

            PreviewKeyDown += new KeyEventHandler(W_PreviewKeyDown);
        }

        private void CrateTextBox()
        {
            Field = GetField();
            Field.ToolTip = "Enter - Confirmar\nEsc - Sair";
            Field.CharacterCasing = CharacterCasing.Upper;
            Field.Name = "txtEdit";
            Field.PreviewKeyDown += new KeyEventHandler(textbox_PreviewKeyDown);
            stkTextBox.Children.Add(Field);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            borderMain.BorderBrush = new SolidColorBrush(Colors.Teal);
            borderMain.BorderThickness = new Thickness(1);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            if (closed == false && validating == false && f2pressed == false)
            {
                //borderMain.BorderBrush = new SolidColorBrush(Colors.Red);
                //borderMain.BorderThickness = new Thickness(2);
                if (ValidCloseInDeactivated())
                {
                    ConfirmedChanges();
                    Close();
                }
            }
        }

        private bool ValidCloseInDeactivated()
        {
            if (!CheckInputIsValid()) return false;
            if (CheckIsExistsEventValidation())
                return Valid();
            return true;
        }

        private TextBox GetField()
        {
            var textbox = new TextBox();
            if (_column as TextColumnEdit is null)
            {
                if (_propertyType == typeof(decimal))
                    textbox = new TextBoxDecimal() { QuantityDecimais = Decimais(_oldValue) };
                else if (_propertyType == typeof(DateTime))
                    textbox = new TextBoxDate();
                else if (_propertyType == typeof(int) || _propertyType == typeof(Int16) || _propertyType == typeof(Int32) || _propertyType == typeof(Int64))
                    textbox = new TextBoxInt();
            }
            else
            {
                textbox = FieldEditCustom(_column as TextColumnEdit);
                var col = _column as TextColumnEdit;
                if (col.MaxLength != TextColumnEdit.MaxLengthDefault)
                    textbox.MaxLength = col.MaxLength;
            }
            return textbox;
        }

        internal void DefineStyleTextBox(DataGrid dataGrid, DataGridRow dataGridRow)
        {
            Field.FontSize = dataGrid.FontSize;
            Field.Foreground = dataGrid.Foreground;
            Field.FontWeight = dataGrid.FontWeight;
            Field.FontFamily = dataGrid.FontFamily;
            if (dataGridRow.ActualHeight > 0)
                Field.Height = dataGridRow.ActualHeight;
        }

        private TextBox FieldEditCustom(TextColumnEdit column)
        {
            var colDecimal = column as TextColumnEditDecimal;
            if (colDecimal != null)
                return new TextBoxDecimal() { QuantityDecimais = colDecimal.Decimais };

            var colDate = column as TextColumnEditDate;
            if (colDate != null)
                return new TextBoxDate();

            var colInteger = column as TextColumnEditInteger;
            if (colInteger != null)
                return new TextBoxInt();

            var colMask = column as TextColumnEditMask;
            if (colMask != null)
                return new TextBoxMask() { Mask = colMask.Mask };

            if (_propertyType == typeof(decimal))
                return new TextBoxDecimal() { QuantityDecimais = Decimais(_oldValue) };
            else if (_propertyType == typeof(DateTime))
                return new TextBoxDate();
            else if (_propertyType == typeof(int) || _propertyType == typeof(Int16) || _propertyType == typeof(Int32) || _propertyType == typeof(Int64))
                return new TextBoxInt();

            return new TextBox();
        }

        private int Decimais(object oldValue)
        {
            if (oldValue == null)
                return 2;

            var split = oldValue.ToString().Split(',');
            if (split.Length > 1)
                return split[1].Length;
            return 2;
        }

        private void W_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                closed = true;
                Close();
            }
            else if (e.Key == Key.F2)
            {
                try
                {
                    f2pressed = true;
                    var value = OnEventF2EventHandler();
                    if (value != null)
                        Field.Text = value;
                }
                catch
                {
                }
                finally
                {
                    f2pressed = false;
                }
            }
        }

        public Result Input()
        {
            ShowDialog();
            return new Result()
            {
                PressedEnter = pressedEnter,
                Success = success,
                NewValue = Field.Text,
                OldValue = _oldValue.ToString(),
                Changes = !Field.Text.Equals(_oldValue)
            };
        }

        private void textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                pressedEnter = true;
                ConfirmedChanges();
            }
        }

        private void ConfirmedChanges()
        {
            if (Valid() == false)
            {
                Field.DefineFocusSelectAll();
            }
            else
            {
                success = true;
                closed = true;
                Close();
            }
        }

        private bool Valid()
        {
            if (CheckInputIsValid() == false) return Focus(Field);
            if (OnValidation() == false) return Focus(Field);
            return true;
        }

        private bool Focus(TextBox text)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Activate();
                text.Focus();
            }));
            return false;
        }

        private bool CheckInputIsValid()
        {
            if (CheckTypeWithInputText.CheckInputIsValid(_propertyType, Field.Text) == false)
            {
                Field.Background = new SolidColorBrush(Colors.Red);
                Field.Foreground = new SolidColorBrush(Colors.White);

                return false;
            }
            return true;
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        public bool OnValidation()
        {
            try
            {
                validating = true;

                if (_column == null) return true;

                var eventDelegate = GetEventDelegateValidate();
                if (eventDelegate != null)
                {
                    foreach (var eventHandler in eventDelegate.GetInvocationList())
                    {
                        var eventArgs = new ValidateEventArgs()
                        {
                            NewValue = Field.Text,
                            OldValue = _oldValue
                        };

                        var result = (bool)eventHandler.Method.Invoke(
                                eventHandler.Target, new object[] { this, eventArgs });

                        if (result == false)
                            return result;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                validating = false;
            }
        }

        private bool CheckIsExistsEventValidation()
        {
            var eventDelegate = GetEventDelegateValidate();
            return eventDelegate != null;
        }

        private MulticastDelegate GetEventDelegateValidate()
        {
            Type type = _column.GetType();

            var field = type.GetField("EventValidation", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
                return null;

            var eventDelegate = field.GetValue(_column) as MulticastDelegate;
            return eventDelegate;
        }

        private string OnEventF2EventHandler()
        {
            Type type = _column.GetType();

            var field = type.GetField("EventF2EventHandler", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
                return string.Empty;

            var eventDelegate = field.GetValue(_column) as MulticastDelegate;
            if (eventDelegate == null)
                return string.Empty;

            var eventHandler = eventDelegate.GetInvocationList().FirstOrDefault();
            if (eventHandler == null)
                return string.Empty;

            var result = (string)eventHandler.Method.Invoke(
                    eventHandler.Target, new object[] { this, EventArgs.Empty });
            return result;
        }
    }
}