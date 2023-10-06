using EditCellDataGrid.EventsArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EditCellDataGrid
{
    public partial class MainWindow : Window
    {
        private List<Produto> items = null;

        public MainWindow()
        {
            InitializeComponent();

            var random = new Random();
            items = new List<Produto>();
            for (int i = 1; i <= 100; i++)
            {
                var price = random.Next(10, 150);
                var quantity = random.Next(1, 5);
                items.Add(new Produto()
                {
                    Id = i,
                    Description = Guid.NewGuid().ToString(),
                    Price = price,
                    Quantity = quantity,
                    Total = price * quantity,
                    Date = DateTime.Now.Date,
                    Cpf = "000.000.000-00",
                    Obs = Guid.NewGuid().ToString()
                });
            }

            dgv.ItemsSource = new List<Produto>();
            dgv.ItemsSource = items;
            dgv.MangerEdit<Produto>();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }

        /// <summary>
        /// Validation example column Id
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool DataGridTextColumn_ValidationEdit(object sender, ValidateEventArgs e)
        {
            if (e.NewValue is null || e.NewValue.ToString().CheckNullOrEmpty())
                return Message("Informe o nome Id");

            var checkout = items.FirstOrDefault(c => c.Id == e.NewValue.ToString().CastToInt16());
            if (checkout == null)
                return true;
            return Message("Id já existe na lista");
        }        

        private void DataGridTextColumnEditCell_DefineNewValue(object sender, EditCellEventArgs result)
        {
            var item = dgv.SelectedItem as Produto;
            item.Price = Convert.ToDecimal(result.NewValue);
        }

        private bool Message(string message, MessageBoxImage messageBoxImage = MessageBoxImage.Information)
        {
            MessageBox.Show(message, "Atenção", MessageBoxButton.OK, messageBoxImage);
            return false;
        }

        private void DataGridTextColumnEditCell_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F2)
            {
                MessageBox.Show("Abra sua consulta aqui", "Atenção");
            }
        }
    }
}