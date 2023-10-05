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
        public string Value { get; set; }
        public bool Changes { get; set; }
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
        private readonly DataGridTextColumn _column;
        private readonly object _oldText;
        public readonly TextBox textbox = null;
        public readonly Type _propertyType = null;

        public EditCell(Window owner, string value, TypeInput typeInput, DataGridTextColumn column, Type propertyType)
        {
            InitializeComponent();

            Owner = owner;
            _column = column;
            _propertyType = propertyType;

            textbox = new TextBox();
            if (propertyType == typeof(decimal))
                textbox = new TextBoxDecimal();
            else if (propertyType == typeof(DateTime))
                textbox = new TextBoxDate();
            else if (propertyType == typeof(int) || propertyType == typeof(Int16) || propertyType == typeof(Int32) || propertyType == typeof(Int64))
                textbox = new TextBoxInt();

            textbox.Name = "txtEdit";
            textbox.PreviewKeyDown += new KeyEventHandler(textbox_PreviewKeyDown);
            stkTextBox.Children.Add(textbox);

            _oldText = value;
            value = value.ToUpper();

            textbox.Text = value;
            textbox.Focus();

            if (typeInput == TypeInput.KeyboardDevice)
                textbox.SelectionStart = value.Length;
            else
                textbox.DefineFocusSelectAll();

            PreviewKeyDown += new KeyEventHandler(W_PreviewKeyDown);
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
                Value = textbox.Text,
                Changes = !textbox.Equals(_oldText)
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
                    OldValue = _oldText,
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