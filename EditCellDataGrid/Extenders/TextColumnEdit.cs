using EditCellDataGrid.Delegates;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace EditCellDataGrid.Extenders
{
    public class TextColumnEdit : DataGridTextColumn
    {
        public event EditCellValidationEventHandler Validation;
        public event DefineNewValueEventHandler DefineNewValue;
        public event KeyEventHandler PreviewKeyDown;
        public event NewValueConfirmedEventHandler NewValueConfirmed;

        public static int MaxLengthDefault = 9999;

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(TextColumnEdit), new PropertyMetadata(MaxLengthDefault));
    }

    public class TextColumnEditDecimal : TextColumnEdit
    {
        public int Decimais
        {
            get { return (int)GetValue(DecimaisProperty); }
            set { SetValue(DecimaisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Decimais.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecimaisProperty =
            DependencyProperty.Register("Decimais", typeof(int), typeof(TextColumnEditDecimal), new PropertyMetadata(2));
    }

    public class TextColumnEditDate : TextColumnEdit
    {
    }

    public class TextColumnEditInteger : TextColumnEdit
    {
    }

    public class TextColumnEditMask : TextColumnEdit
    {
        public string Mask
        {
            get { return (string)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Mask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.Register("Mask", typeof(string), typeof(TextColumnEditMask), new PropertyMetadata(""));
    }
}