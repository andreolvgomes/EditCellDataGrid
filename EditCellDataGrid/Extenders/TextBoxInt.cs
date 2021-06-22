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
    public class TextBoxInt : TextBox
    {
        public int ValueMax
        {
            get { return (int)GetValue(ValorMaximoPermitidoProperty); }
            set { SetValue(ValorMaximoPermitidoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValorMaximoPermitidoProperty =
            DependencyProperty.Register("ValueMax", typeof(int), typeof(TextBoxInt), new PropertyMetadata(0));

        public bool AllowNegative
        {
            get { return (bool)GetValue(AceitaNegativoProperty); }
            set { SetValue(AceitaNegativoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowNegative.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AceitaNegativoProperty =
            DependencyProperty.Register("AllowNegative", typeof(bool), typeof(TextBoxInt), new PropertyMetadata(false));


        public TextBoxInt()
        {
            this.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(T_PreviewTextInput);
            this.TextAlignment = System.Windows.TextAlignment.Right;
        }

        private void T_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            bool isDigitValido = this.IsDigit(e.Text) || (this.AllowNegative && e.Text == NumberFormatInfo.CurrentInfo.NegativeSign); //Para aceitar negativo
            e.Handled = !isDigitValido;
            if (isDigitValido)
            {
                string _text = "";
                int _selectionStart = 0;
                int position = this.SelectionStart;

                if (this.SelectionLength > 0)
                {
                    this.Text = this.Text.Remove(this.SelectionStart, this.SelectionLength);
                    this.Text = this.Text.Insert(position, e.Text);
                    _text = this.Text;
                }
                else if (this.SelectionStart == this.Text.Length)
                {
                    _text = this.Text + e.Text;
                }
                else if (this.Text.Length > this.SelectionStart)
                {
                    _selectionStart = this.SelectionStart + 1;
                    this.Text = this.Text.Insert(position, e.Text);
                    _text = this.Text;
                }
                if (this.ValidaValorMaximo(_text))
                {
                    if (this.MaxLength > 0)
                        this.Text = _text.SubstringBest(0, this.MaxLength);
                    else
                        this.Text = _text;
                    this.SelectionStart = (_selectionStart == 0) ? this.Text.Length : _selectionStart;
                }
                e.Handled = true;
            }
        }

        private bool ValidaValorMaximo(string text)
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

        private bool IsDigit(string p)
        {
            foreach (char c in p)
                if (!char.IsDigit(c))
                    return false;
            return true;
        }
    }
}
