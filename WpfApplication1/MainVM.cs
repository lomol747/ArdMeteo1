using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArdMeteo
{
    class MainVM : INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _TextTest;
        public string TextTest
        {
            get { return "Пистец"; }
            set
            {
                _TextTest = "Охуеть";
                OnPropertyChanged("Number3"); // уведомление View о том, что изменилась сумма
            }
        }

        private string _TextTest2;
        public string TextTest2
        {
            get { return "Пистец2"; }
            set
            {
                _TextTest = "Охуеть2";
            }
        }

        private string _TextTest3;
        public string TextTest3
        {
            get { return "Пистец3"; }
            set
            {
                _TextTest = "Охуеть3";
            }
        }

        private double _number1;
        public double Number1
        {
            get { return _number1; }
            set
            {
                _number1 = value;
                OnPropertyChanged("Number3"); // уведомление View о том, что изменилась сумма
            }
        }

        private double _number2;
        public double Number2
        {
            get { return _number2; }
            set { _number1 = value; OnPropertyChanged("Number3"); }
        }

        //свойство только для чтения, оно считывается View каждый раз, когда обновляется Number1 или Number2
        public double Number3 => Model.GetSumOf(Number1, Number2);

        public string TextTest31 { get => _TextTest3; set => _TextTest3 = value; }
    }
}
