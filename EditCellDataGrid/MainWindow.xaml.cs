using EditCellDataGrid.Edit;
using EditCellDataGrid.Extenders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EditCellDataGrid
{
    public class Produto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
    }

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
                    Date = DateTime.Now.Date
                });
            }

            dgv.ItemsSource = new List<Produto>();
            dgv.ItemsSource = items;

            DataGridCellEdit.BeginEdit(dgv);
        }

        /// <summary>
        /// Validation example column Id
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool DataGridTextColumnValidation_ValidationEdit(object sender, EditCellEventArgs e)
        {
            if (e.NewValue is null || e.NewValue.ToString().NullOrEmpty())
                return Message("Informe o nome Id");

            var checkout = items.FirstOrDefault(c => c.Id == e.NewValue.ToString().ToInt16());
            if (checkout == null)
                return true;
            return Message("Id já existe na lista");
        }

        private bool Message(string message, MessageBoxImage messageBoxImage = MessageBoxImage.Information)
        {
            MessageBox.Show(message, "Atenção", MessageBoxButton.OK, messageBoxImage);
            return false;
        }
    }
}