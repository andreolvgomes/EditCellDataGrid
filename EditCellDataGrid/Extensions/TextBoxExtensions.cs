using System.Windows.Controls;

namespace EditCellDataGrid.Extensions
{
    public static class TextBoxExtensions
    {
        public static bool DefineFocusSelectAll(this TextBox textbox)
        {
            textbox.Focus();
            textbox.SelectAll();
            return false;
        }
    }
}