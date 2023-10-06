using System;
using System.Windows.Controls;

namespace EditCellDataGrid.Extenders
{
    public class TextBoxDate : TextBox
    {
        public TextBoxDate()
        {
            this.MaxLength = 10;
            this.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(T_PreviewTextInput);
        }

        private void T_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            bool prossegue = true;

            if (this.MaxLength > 0)
                if (!(this.Text.Length < this.MaxLength))
                    prossegue = false;

            if (prossegue)
            {
                bool valid = ValidaCaracter(e.Text);
                e.Handled = !valid;
                if (valid)
                {
                    int _selectionStart = 0;
                    string str = "";
                    int position = this.SelectionStart;

                    if (this.SelectionLength > 0)
                    {
                        this.Text = this.Text.Remove(this.SelectionStart, this.SelectionLength);
                        this.Text = this.Text.Insert(position, e.Text);
                        str = this.Text;
                    }
                    else if (this.SelectionStart == this.Text.Length)
                    {
                        str = this.Text + e.Text;
                    }
                    else if (this.Text.Length > this.SelectionStart)
                    {
                        _selectionStart = this.SelectionStart + 1;
                        this.Text = this.Text.Insert(position, e.Text);
                        str = this.Text;
                    }
                    this.Text = this.FormatDate(str);
                    if (this.Text.Length == 6)
                    {
                        this.Text += DateTime.Now.Year;
                        this.SelectionStart = 6;
                        this.SelectionLength = 4;
                    }
                    else
                    {
                        this.SelectionStart = (_selectionStart == 0) ? this.Text.Length : _selectionStart;
                    }
                    e.Handled = true;
                }
            }
        }

        private string FormatDate(string str)
        {
            string result = "";
            str = RemoveCaracteresEspeciais(str);
            for (int i = 0; i < str.Length; i++)
            {
                result += str[i];
                if (i == 1 || i == 3) result += "/";
            }
            if (result.Length > this.MaxLength)
                result = result.Substring(0, this.MaxLength);

            return result;
        }

        private string RemoveCaracteresEspeciais(string retorno)
        {
            foreach (string caracter in new string[] { ".", "/", "-", " ", "(", ")" })
                retorno = retorno.Replace(caracter, "");
            return retorno;
        }

        private bool ValidaCaracter(string text)
        {
            foreach (char c in text)
                if (!char.IsDigit(c))
                    return false;
            return true;
        }
    }
}
