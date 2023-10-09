using System;
using System.ComponentModel;

namespace EditCellDataGrid
{
    public class Produto : INotifyPropertyChanged
    {
        private bool _IsChecked;

        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                if (_IsChecked != value)
                {
                    _IsChecked = value;
                    OnPropertyChanged("IsChecked");
                }
            }
        }

        private int _Id;

        public int Id
        {
            get { return _Id; }
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        private string _Description;

        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        private decimal _Price;

        public decimal Price
        {
            get { return _Price; }
            set
            {
                if (_Price != value)
                {
                    _Price = value;
                    OnPropertyChanged("Price");
                }
            }
        }

        private decimal _Quantity;

        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {
                if (_Quantity != value)
                {
                    _Quantity = value;
                    OnPropertyChanged("Quantity");
                }
            }
        }

        private decimal _Total;

        public decimal Total
        {
            get { return _Total; }
            set
            {
                if (_Total != value)
                {
                    _Total = value;
                    OnPropertyChanged("Total");
                }
            }
        }

        private DateTime _Date;

        public DateTime Date
        {
            get { return _Date; }
            set
            {
                if (_Date != value)
                {
                    _Date = value;
                    OnPropertyChanged("Date");
                }
            }
        }

        private string _Cpf;

        public string Cpf
        {
            get { return _Cpf; }
            set
            {
                if (_Cpf != value)
                {
                    _Cpf = value;
                    OnPropertyChanged("Cpf");
                }
            }
        }

        private string _Obs;

        public string Obs
        {
            get { return _Obs; }
            set
            {
                if (_Obs != value)
                {
                    _Obs = value;
                    OnPropertyChanged("Obs");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}