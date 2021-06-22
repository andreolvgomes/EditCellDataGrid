using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EditCellDataGrid.Extenders
{
    public class TextBoxDecimal : TextBox
    {
        public bool AllowNegative
        {
            get { return (bool)GetValue(AceitaNegativoProperty); }
            set { SetValue(AceitaNegativoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowNegative.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AceitaNegativoProperty =
            DependencyProperty.Register("AllowNegative", typeof(bool), typeof(TextBoxDecimal), new PropertyMetadata(false));


        public decimal ValueMax
        {
            get { return (decimal)GetValue(ValorMaximoPermitidoProperty); }
            set { SetValue(ValorMaximoPermitidoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValorMaximoPermitidoProperty =
            DependencyProperty.Register("ValueMax", typeof(decimal), typeof(TextBoxDecimal), new PropertyMetadata((decimal)0.00));


        public int QuantityDecimais
        {
            get { return (int)GetValue(CasasDecimalProperty); }
            set { SetValue(CasasDecimalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CasasDecimal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CasasDecimalProperty =
            DependencyProperty.Register("QuantityDecimais", typeof(int), typeof(TextBoxDecimal), new PropertyMetadata(2));


        public int DecimalsIfEmpty
        {
            get { return (int)GetValue(DecimalsIfEmptyProperty); }
            set { SetValue(DecimalsIfEmptyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecimalsIfEmpty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecimalsIfEmptyProperty =
            DependencyProperty.Register("DecimalsIfEmpty", typeof(int), typeof(TextBoxDecimal), new PropertyMetadata(0));

        private bool prossegue = true;

        public TextBoxDecimal()
        {
            this.PreviewTextInput += TextBox_PreviewTextInput;

            this.LostKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(TextBox_LostKeyboardFocus);
            DataObject.AddPastingHandler(this, TextBoxPastingEventHandler);

            this.PreencheTextBox();
            this.TextAlignment = TextAlignment.Right;
            this.ValidateTextBox();
        }

        private void TextBox_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            this.PreencheTextBox();
        }

        private void PreencheTextBox()
        {
            if (string.IsNullOrEmpty(Text))
                Text = "0,00";

            if (!this.Text.CheckDecimal())
                this.Text = "0," + "0".PadRight(this.QuantityDecimais, '0');
            else
                this.Text = (Convert.ToDecimal(this.Text)).ToString(string.Format("0.{0}", "".PadRight(this.QuantityDecimais, '0'))).Replace(".", "");
        }

        private void ValidateTextBox()
        {
            this.Text = this.ValidateValue(this.Text);
        }

        private void TextBoxPastingEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            string clipboard = e.DataObject.GetData(typeof(string)) as string;
            clipboard = ValidateValue(clipboard);
            if (!string.IsNullOrEmpty(clipboard))
            {
                this.Text = clipboard;
            }
            e.CancelCommand();
            e.Handled = true;
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (this.IsReadOnly)
            {
                e.Handled = true;
                return;
            }
            prossegue = true;

            if (this.MaxLength > 0)
                if (this.Text.Length < this.MaxLength)
                    prossegue = true;
                else
                    prossegue = false;

            if (prossegue)
            {
                bool isValid = this.IsSymbolValid(e.Text);
                e.Handled = !isValid;
                if (!isValid) return;//break

                isValid = this.IsValidComma(this, e.Text);
                e.Handled = !isValid;
                if (isValid)
                {
                    int caret = this.CaretIndex;
                    string text = this.Text;
                    bool textInserted = false;
                    int selectionLength = 0;
                    string textbkp = text;

                    if (this.SelectionLength > 0)
                    {
                        text = text.Substring(0, this.SelectionStart) + text.Substring(this.SelectionStart + this.SelectionLength);
                        caret = this.SelectionStart;
                    }

                    if (e.Text == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    {
                        while (true)
                        {
                            int ind = text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                            if (ind == -1)
                                break;

                            text = text.Substring(0, ind) + text.Substring(ind + 1);
                            if (caret > ind)
                                caret--;
                        }

                        if (caret == 0)
                        {
                            text = "0" + text;
                            caret++;
                        }
                        else
                        {
                            if (caret == 1 && string.Empty + text[0] == NumberFormatInfo.CurrentInfo.NegativeSign)
                            {
                                text = NumberFormatInfo.CurrentInfo.NegativeSign + "0" + text.Substring(1);
                                caret++;
                            }
                        }

                        if (caret == text.Length)
                        {
                            selectionLength = 1;
                            textInserted = true;
                            text = text + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0";
                            caret++;
                        }
                    }
                    else if (e.Text == NumberFormatInfo.CurrentInfo.NegativeSign)
                    {
                        textInserted = true;
                        if (this.Text.Contains(NumberFormatInfo.CurrentInfo.NegativeSign))
                        {
                            text = text.Replace(NumberFormatInfo.CurrentInfo.NegativeSign, string.Empty);
                            if (caret != 0)
                                caret--;
                        }
                        else
                        {
                            text = NumberFormatInfo.CurrentInfo.NegativeSign + this.Text;
                            caret++;
                        }
                    }

                    if (!textInserted)
                    {
                        text = text.Substring(0, caret) + e.Text + ((caret < this.Text.Length) ? text.Substring(caret) : string.Empty);
                        caret++;
                    }

                    while (text.Length > 1 && text[0] == '0' && string.Empty + text[1] != NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    {
                        text = text.Substring(1);
                        if (caret > 0)
                            caret--;
                    }

                    while (text.Length > 2 && string.Empty + text[0] == NumberFormatInfo.CurrentInfo.NegativeSign && text[1] == '0' && string.Empty + text[2] != NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    {
                        text = NumberFormatInfo.CurrentInfo.NegativeSign + text.Substring(2);
                        if (caret > 1)
                            caret--;
                    }

                    if (caret > text.Length)
                        caret = text.Length;

                    if (ValidValueMax(text))
                    {
                        this.Text = text;
                        this.CaretIndex = caret;
                        this.SelectionStart = caret;
                        this.SelectionLength = selectionLength;
                    }
                    e.Handled = true;
                }
            }
        }

        private bool ValidValueMax(string text)
        {
            if (ValueMax <= 0)
                return true;
            decimal value = 0;
            if (decimal.TryParse(text, out value))
            {
                if (value > ValueMax)
                    return false;
            }
            return true;
        }

        private bool IsValidComma(TextBox box, string str)
        {
            if (str == NumberFormatInfo.CurrentInfo.NegativeSign)
                if (this.AllowNegative) return true;
                else return false;


            if (str != NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                foreach (char ch in str)
                {
                    if (!Char.IsDigit(ch)) return false;
                }

            int pVirgula = box.Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
            if (str == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator && pVirgula != -1)
            {
                return false;
            }
            else
            {
                if (pVirgula != -1)
                {
                    int tamanho = box.Text.Substring(pVirgula + 1, box.Text.Length - (pVirgula + 1)).Length;

                    if (box.CaretIndex > pVirgula && tamanho == this.QuantityDecimais)
                        return false;
                }
            }
            return true;
        }

        private string ValidateValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            value = value.Trim();
            try
            {
                Convert.ToDouble(value);
                return value;
            }
            catch
            {
            }
            return value;
        }

        private bool IsSymbolValid(string str)
        {
            if (str == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator || str == NumberFormatInfo.CurrentInfo.NegativeSign)
                return true;

            foreach (char ch in str)
            {
                if (!Char.IsDigit(ch))
                    return false;
            }
            return true;
        }
    }
}
