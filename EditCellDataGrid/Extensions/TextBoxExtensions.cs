using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace System
{
    public static class TextBoxExtensions
    {
        public static bool FocusSelectAll(this TextBox textbox)
        {
            textbox.Focus();
            textbox.SelectAll();
            return false;
        }
    }
}
