using EditCellDataGrid.EventsArgs;
using EditCellDataGrid.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EditCellDataGrid
{
    public class Result
    {
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
        private bool success = false;
        private readonly DataGridColumn _column;
        private readonly string _oldValue;
        public readonly TextBox textbox = null;
        public readonly Type _propertyType = null;

        public EditCell(Window owner, string oldValue, string value, TypeInput typeInput, DataGridColumn column, Type propertyType)
        {
            InitializeComponent();

            Owner = owner;
            _column = column;
            _propertyType = propertyType;
            _oldValue = oldValue;

            textbox = GetField();
            textbox.Name = "txtEdit";
            textbox.PreviewKeyDown += new KeyEventHandler(textbox_PreviewKeyDown);
            stkTextBox.Children.Add(textbox);
            
            textbox.Text = value;
            textbox.Focus();

            if (typeInput == TypeInput.KeyboardDevice)
                textbox.SelectionStart = value.Length;
            else
                textbox.DefineFocusSelectAll();

            PreviewKeyDown += new KeyEventHandler(W_PreviewKeyDown);
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
                Close();
        }

        public Result Get()
        {
            ShowDialog();
            return new Result()
            {
                Success = success,
                NewValue = textbox.Text,
                OldValue = _oldValue.ToString(),
                Changes = !textbox.Equals(_oldValue)
            };
        }

        private void textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Ok();
            }
        }

        private void Ok()
        {
            if (Valid() == false)
            {
                textbox.DefineFocusSelectAll();
            }
            else
            {
                success = true;
                Close();
            }
        }

        private bool Valid()
        {
            if (CheckDateTime() == false)
                return false;
            return OnValidation();
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
            if (_column == null) return true;

            Type type = _column.GetType();

            var field = type.GetField("Validation", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) return true;

            var eventDelegate = field.GetValue(_column) as MulticastDelegate;
            if (eventDelegate == null)
                return true;

            foreach (var eventHandler in eventDelegate.GetInvocationList())
            {
                var eventArgs = new EditCellEventArgs()
                {
                    OldValue = _oldValue,
                    NewValue = textbox.Text
                };

                var result = (bool)eventHandler.Method.Invoke(
                        eventHandler.Target, new object[] { _column, eventArgs });

                if (result == false)
                    return result;
            }
            return true;
        }
    }
}