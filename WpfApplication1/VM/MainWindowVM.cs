using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ArdMeteo
{
    class MainWindowVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private SerialPortModel serialPortM = new SerialPortModel();    //инициализация MODEL

        // поля участвующие во Views
        public string Temp { get; set; }
        public string Press { get; set; }

        Dispatcher disp;

        //***************************************************************************************
        //Коллекция для графиков
        private ObservableCollection<KeyValuePair<string, double>> _dataCurrTemp = new ObservableCollection<KeyValuePair<string, double>>();
        public ObservableCollection<KeyValuePair<string, double>> dataCurrTemp { get { return _dataCurrTemp; } } //коллекция текущей температруы
        //public ObservableCollection<KeyValuePair<string, double>> dataCurrTemp = new ObservableCollection<KeyValuePair<string, double>>();
        ObservableCollection<KeyValuePair<string, double>> dataCurrPress = new ObservableCollection<KeyValuePair<string, double>>();    //коллекция текущего давления
        ObservableCollection<KeyValuePair<string, double>> dataTemp = new ObservableCollection<KeyValuePair<string, double>>();    //коллекция температуры
        ObservableCollection<KeyValuePair<string, double>> dataPress = new ObservableCollection<KeyValuePair<string, double>>();    //коллекция давления

        ObservableCollection<KeyValuePair<string, double>> dataTest = new ObservableCollection<KeyValuePair<string, double>>();    //коллекция тестовая
        //***************************************************************************************

        public MainWindowVM()
        {
            //подписка на события при инициализации
            serialPortM.eventTemp += setViewTemp;
            //serialPortM.eventPress += setViewPress;

            //init(_dataCurrTemp);

            disp = Dispatcher.CurrentDispatcher;  //получаем текущий поток для выполнения записи в график

            int ds = 0;
        }

        protected virtual void OnPropertyChanged(string propertyName)   //оповещение View
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void init(ObservableCollection<KeyValuePair<string, double>> collect)    //отладочный код
        {
            collect.Add(new KeyValuePair<string, double>("Dog", 30));
            collect.Add(new KeyValuePair<string, double>("Cat", 25));
            collect.Add(new KeyValuePair<string, double>("Rat", 5));
            collect.Add(new KeyValuePair<string, double>("Hampster", 8));
            collect.Add(new KeyValuePair<string, double>("Rabbit", 12));

            //OnPropertyChanged("dataCurrTemp");
        }

        //************************************************************
        //Получение данных из Model
        public void setViewTemp ()      //получение температуры
        {
            Temp = serialPortM._temp;   //получение данных

            disp.BeginInvoke(new recCollectint(recordChartCollection), _dataCurrTemp, Temp);
            //recordChartCollection(_dataCurrTemp, Temp);

            OnPropertyChanged("Temp");  //уведомление View об изменении
        }
        public void setViewPress()
        {
            Press = serialPortM._press; //получение давления
            recordChartCollection(dataCurrPress, Press);
            OnPropertyChanged("Press");

        }
        //*************************************************************
        //ДИАГРАММЫ

        
        //так как запись в коллекцию вызывается в потоке получения из ком порта, то вызывается исключение.
        //Для выполнения маршализации в текущем потоке, требуется получение текущего и вызов в нём BeginInvoke с соответствующим делегатом
        delegate void recCollectint(ObservableCollection<KeyValuePair<string, double>> collection, string value);

        //Запись в коллекцию
        private void recordChartCollection(ObservableCollection<KeyValuePair<string, double>> collection, string value)
        {
            if (value != null)
            {
                string date = DateTime.Now.ToString();      //получение текущей даты

                value = value.Trim(new char[] { '\t' });    //вырезание последнего символа. Особенность получаемого символа. ИЗМЕНИТСЯ
                value = value.Replace(".", ",");            //замена точки на запятую для парсирования в double

                double val = Double.Parse(value);           //парсирование в double
                date = date.Substring(11);                  //обрезание лишних символов. Остаётся только дата

                collection.Add(new KeyValuePair<string, double>(date, val));    //добавление в коллекцию
                int g = 0;
            }
        }

        /// Очищение коллекции
        private void removeChartCollection(ObservableCollection<KeyValuePair<string, double>> collection)
        {
            collection.Clear(); //двойной, иначе визуально удаляется не полностью, хотя коллекция пуста
            collection.Clear();
        }


    }
}
