using System.Windows;
using System;
using System.Windows.Controls;
using EditCellDataGrid.Extensions;

namespace EditCellDataGrid.Extenders
{
    public class TextBoxMask : TextBox
    {
        public bool IgnoreMask
        {
            get { return (bool)GetValue(IgnoreMaskProperty); }
            set { SetValue(IgnoreMaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IgnoreMask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IgnoreMaskProperty =
            DependencyProperty.Register("IgnoreMask", typeof(bool), typeof(TextBoxMask), new PropertyMetadata(false));


        public string Mask
        {
            get { return (string)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Mask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.Register("Mask", typeof(string), typeof(TextBoxMask), new PropertyMetadata("0000"));


        public TextBoxMask()
        {
            this.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(T_PreviewTextInput);
        }

        private void T_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (this.IgnoreMask)
                return;

            string _mask = this.Mask + "0";

            if (this.Text.Length + e.Text.Length > _mask.Length - 1 && this.SelectionLength == 0)
            {
                e.Handled = true;
                return;
            }

            bool valid = DigitValid(e.Text);
            e.Handled = !valid;

            if (valid)
            {
                int _selectionStart = 0;
                int position = this.SelectionStart;
                string textInfo = "";

                if (this.SelectionLength > 0)
                {
                    _selectionStart = this.SelectionStart + this.SelectionLength;
                    this.Text = this.Text.Remove(this.SelectionStart, this.SelectionLength);
                    this.Text = this.Text.Insert(position, e.Text);
                    textInfo = this.Text;
                }
                else if (this.SelectionStart == this.Text.Length)
                {
                    textInfo = this.Text + e.Text;
                }
                else if (this.Text.Length > this.SelectionStart)
                {
                    _selectionStart = this.SelectionStart + 1;
                    this.Text = this.Text.Insert(position, e.Text);
                    textInfo = this.Text;
                }

                this.Text = string.Empty;
                int i = 0;
                foreach (char text in textInfo)
                {
                    i++;
                    if (char.IsDigit(text))
                    {
                        this.Text += text;
                        if (_mask[i].ToString() != "0")
                        {
                            this.Text += _mask[i].ToString();
                        }
                    }
                }
                if (i > 0 && !this.Text.CheckNullOrEmpty())
                {
                    string c = this.Text.SubstringBest(this.Text.Length - 1, 1);
                    foreach (char x in c)
                    {
                        if (!char.IsDigit(x))
                            this.Text = this.Text.SubstringBest(0, this.Text.Length - 1);
                    }
                }
                this.SelectionStart = (_selectionStart == 0) ? this.Text.Length : _selectionStart;
                e.Handled = true;
            }
        }

        private static bool DigitValid(string text)
        {
            foreach (char c in text)
                if (!char.IsDigit(c)) return false;
            return true;
        }
    }
}
