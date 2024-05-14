using EditCellDataGrid.Extensions;
using EditCellDataGrid.EventsArgs;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Linq;
using System;

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
                    Obs = Guid.NewGuid().ToString(),
                    Lucro = 25
                });
            }

            dgv.ItemsSource = new List<Produto>();
            dgv.ItemsSource = items;

            var dataGridCellEdit = dgv.ManagerEdit<Produto>(settings: new CellEditSettings() { MoveFocusNextRowAfterConfirmedWithEnter = false });
            dataGridCellEdit.EventDataGridValueChanged += DataGridValueChanged;

            sliderFontSize.Value = dgv.FontSize;
        }

        private void DataGridValueChanged(object sender, DataGridlValueChangedEventArgs<Produto> e)
        {

        }

        /// <summary>
        /// EventValidation example column Id
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
            //MessageBox.Show("Evento valor definido Ok");
        }

        private bool Message(string message, MessageBoxImage messageBoxImage = MessageBoxImage.Information)
        {
            MessageBox.Show(message, "Atenção", MessageBoxButton.OK, messageBoxImage);
            return false;
        }

        private void DataGridTextColumnEditCell_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                MessageBox.Show("Abra sua consulta aqui", "Atenção");
            }
        }

        private string TextColumnEdit_EventF2EventHandler(object sender, EventArgs e)
        {
            MessageBox.Show("Event F2 acionado", "Atenção");
            return string.Empty;
        }
    }
}