using EditCellDataGrid.EventsArgs;
using EditCellDataGrid.Extenders;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using System.Windows;
using System;

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
        private bool closed = false;
        private bool validating = false;
        private bool success = false;
        public TextBox textbox = null;

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
            textbox.Text = value.ToUpper();
            textbox.Focus();

            if (typeInput == TypeInput.KeyboardDevice)
                textbox.SelectionStart = value.Length;
            else
                textbox.DefineFocusSelectAll();

            PreviewKeyDown += new KeyEventHandler(W_PreviewKeyDown);
        }

        private void CrateTextBox()
        {
            textbox = GetField();
            textbox.ToolTip = "Enter - Confirmar\nEsc - Sair";
            textbox.CharacterCasing = CharacterCasing.Upper;
            textbox.Name = "txtEdit";
            textbox.PreviewKeyDown += new KeyEventHandler(textbox_PreviewKeyDown);
            stkTextBox.Children.Add(textbox);
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
            if (closed == false && validating == false)
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
            if (!CheckDateTime()) return false;
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
            textbox.FontSize = dataGrid.FontSize;
            textbox.Foreground = dataGrid.Foreground;
            textbox.FontWeight = dataGrid.FontWeight;
            textbox.FontFamily = dataGrid.FontFamily;
            if (dataGridRow.ActualHeight > 0)
                textbox.Height = dataGridRow.ActualHeight;
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
        }

        public Result Get()
        {
            ShowDialog();
            return new Result()
            {
                PressedEnter = pressedEnter,
                Success = success,
                NewValue = textbox.Text,
                OldValue = _oldValue.ToString(),
                Changes = !textbox.Text.Equals(_oldValue)
            };
        }

        private bool pressedEnter = false;
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
                textbox.DefineFocusSelectAll();
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
            if (CheckDateTime() == false)
            {
                textbox.Focus();
                return false;
            }
            if (OnValidation() == false)
            {
                textbox.Focus();
                return false;
            }
            return true;
        }

        private bool CheckDateTime()
        {
            if (_propertyType == typeof(DateTime))
            {
                if (DateTime.TryParse(textbox.Text, out _) == false)
                {
                    textbox.Background = new SolidColorBrush(Colors.Red);
                    textbox.Foreground = new SolidColorBrush(Colors.White);
                    return false;
                }
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
                            NewValue = textbox.Text,
                            OldValue = _oldValue
                        };

                        var result = (bool)eventHandler.Method.Invoke(
                                eventHandler.Target, new object[] { _column, eventArgs });

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

            var field = type.GetField("Validation", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
                return null;

            var eventDelegate = field.GetValue(_column) as MulticastDelegate;
            return eventDelegate;
        }
    }
}