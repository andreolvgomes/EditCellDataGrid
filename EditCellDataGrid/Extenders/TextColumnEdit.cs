using EditCellDataGrid.Delegates;
using System.Windows.Controls;
using System.Windows;

namespace EditCellDataGrid.Extenders
{
    public class TextColumnEdit : DataGridTextColumn
    {
        public event EditCellValidationEventHandler EventValidation;
        public event NewValueConfirmedEventHandler EventCellValueChanged;
        public event EditCellF2EventHandler EventF2EventHandler;
        public event CheckCellCanEditEventHandler EventCheckCellCanEdit;

        public static int MaxLengthDefault = 9999;

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(TextColumnEdit), new PropertyMetadata(MaxLengthDefault));


        public CharacterCasing CharacterCasing
        {
            get { return (CharacterCasing)GetValue(CharacterCasingProperty); }
            set { SetValue(CharacterCasingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CharacterCasing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CharacterCasingProperty =
            DependencyProperty.Register("CharacterCasing", typeof(CharacterCasing), typeof(TextColumnEdit), new PropertyMetadata(CharacterCasing.Upper));


        public string Id
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Id.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(string), typeof(TextColumnEdit), new PropertyMetadata(""));


        public bool NotifyChangeEventDataGrid
        {
            get { return (bool)GetValue(NotifyChangeEventDataGridProperty); }
            set { SetValue(NotifyChangeEventDataGridProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NotifyChangeEventDataGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NotifyChangeEventDataGridProperty =
            DependencyProperty.Register("NotifyChangeEventDataGrid", typeof(bool), typeof(TextColumnEdit), new PropertyMetadata(true));
    }

    public class TextColumnEditDecimal : TextColumnEdit
    {
        public event EditCellValidationEventHandler EventValidation;
        public event NewValueConfirmedEventHandler EventCellValueChanged;
        public event EditCellF2EventHandler EventF2EventHandler;
        public event CheckCellCanEditEventHandler EventCheckCellCanEdit;

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
        public event EditCellValidationEventHandler EventValidation;
        public event NewValueConfirmedEventHandler EventCellValueChanged;
        public event EditCellF2EventHandler EventF2EventHandler;
        public event CheckCellCanEditEventHandler EventCheckCellCanEdit;
    }

    public class TextColumnEditInteger : TextColumnEdit
    {
        public event EditCellValidationEventHandler EventValidation;
        public event NewValueConfirmedEventHandler EventCellValueChanged;
        public event EditCellF2EventHandler EventF2EventHandler;
        public event CheckCellCanEditEventHandler EventCheckCellCanEdit;
    }

    public class TextColumnEditMask : TextColumnEdit
    {
        public event EditCellValidationEventHandler EventValidation;
        public event NewValueConfirmedEventHandler EventCellValueChanged;
        public event EditCellF2EventHandler EventF2EventHandler;
        public event CheckCellCanEditEventHandler EventCheckCellCanEdit;

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