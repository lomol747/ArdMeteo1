using ArdMeteo.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Windows.Input;
using System.IO;

namespace ArdMeteo
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;   //событие изменение
        public event EventHandler CanExecuteChanged;

        SerialPortModel serialPortM = new SerialPortModel();    //инициализация MODEL

        // поля участвующие во Views
        public string Temp  { get; set; }
        public string Press { get; set; }
        public string Hum   { get; set; }
        public string Gas   { get; set; }

        Dispatcher disp;

        //***************************************************************************************
        //Коллекция для графиков
        private ChartTestModel _test = new ChartTestModel();
        public ChartTestModel test { get { return _test; } }

        //private  ObservableCollection<KeyValuePair<string, double>> _dataCurrTemp = new ObservableCollection<KeyValuePair<string, double>>();
        //public ObservableCollection<KeyValuePair<string, double>> dataCurrTemp
        //{
        //    get { return _dataCurrTemp; }
        //    set
        //    {
        //        _dataCurrTemp = value;
        //        OnPropertyChanged("dataCurrTemp");  //уведомление View об изменении
        //    }
        //} //коллекция текущей температруы

        //public ObservableCollection<KeyValuePair<string, double>> dataCurrTemp = new ObservableCollection<KeyValuePair<string, double>>();
        ////public ObservableCollection<KeyValuePair<string, double>> dataCurrTemp { get; } = new ObservableCollection<KeyValuePair<string, double>>();
        //ObservableCollection<KeyValuePair<string, double>> dataCurrPress = new ObservableCollection<KeyValuePair<string, double>>();    //коллекция текущего давления
        //ObservableCollection<KeyValuePair<string, double>> dataTemp = new ObservableCollection<KeyValuePair<string, double>>();    //коллекция температуры
        //ObservableCollection<KeyValuePair<string, double>> dataPress = new ObservableCollection<KeyValuePair<string, double>>();    //коллекция давления

        ////ObservableCollection<KeyValuePair<string, double>> dataTest = new ObservableCollection<KeyValuePair<string, double>>();    //коллекция тестовая
        //public ObservableCollection<KeyValuePair<string, double>> dataTest { get; private set; }
        //***************************************************************************************

        //Обработка кнопки
        private DelegateCommand addCommand;
        public DelegateCommand AddCommand
        {
            get
            {
                return addCommand ??
                    (addCommand = new DelegateCommand(obj =>
                    {
                        timeStart();
                        readFromLog(@"Лог температуры.txt", dataTemp);
                    }));
            }
        }
        
        
        //так как запись в коллекцию вызывается в потоке получения из ком порта, то вызывается исключение.
        //Для выполнения маршализации в текущем потоке, требуется получение текущего и вызов в нём BeginInvoke с соответствующим делегатом
        delegate void recCollection(ObservableCollection<ItemCharStruct> collection, string value);


        public ObservableCollection<Item> Items { get; set; }
        //коллекции точек к графикам
        public ObservableCollection<ItemCharStruct> dataCurrTemp    { get; private set; }
        public ObservableCollection<ItemCharStruct> dataCurrPress   { get; private set; }
        public ObservableCollection<ItemCharStruct> dataCurrHum     { get; private set; }
        public ObservableCollection<ItemCharStruct> dataCurrGas     { get; private set; }
        public ObservableCollection<ItemCharStruct> dataTemp        { get; private set; }
        public ObservableCollection<ItemCharStruct> dataPress       { get; private set; }
        public ObservableCollection<ItemCharStruct> dataHum         { get; private set; }
        public ObservableCollection<ItemCharStruct> dataGas         { get; private set; }



        public PlotModel plotModel { get; private set; }

        public MainWindowVM()
        {
            q = 0;
            Items = new ObservableCollection<Item>();

            //инициализация коллекций
            dataCurrTemp    = new ObservableCollection<ItemCharStruct>();
            dataCurrPress   = new ObservableCollection<ItemCharStruct>();
            dataCurrHum     = new ObservableCollection<ItemCharStruct>();
            dataCurrGas     = new ObservableCollection<ItemCharStruct>();
            dataTemp        = new ObservableCollection<ItemCharStruct>();
            dataPress       = new ObservableCollection<ItemCharStruct>();
            dataHum         = new ObservableCollection<ItemCharStruct>();
            dataGas         = new ObservableCollection<ItemCharStruct>();

            //подписка на события во время инициализации
            serialPortM.eventTemp   += setViewTemp;
            serialPortM.eventPress  += setViewPress;
            serialPortM.eventHum    += setViewHum;
            serialPortM.eventGas    += setViewGas;

            //init(dataTest);


            disp = Dispatcher.CurrentDispatcher;  //получаем текущий поток для выполнения записи в график

            //this.plotModel = new PlotModel { Title = "Examle 1" };
            //this.plotModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
            //timeStart();
            int ds = 0;
        }

        public class Item
        {
            public double Label { get; set; }
            public double Value1 { get; set; }
        }
        


        protected virtual void OnPropertyChanged(string propertyName)   //оповещение View
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void init(ObservableCollection<KeyValuePair<string, double>> collect)    //отладочный код
        {
            DateTime date = new DateTime(2018, 01, 20);
            double dateString1 = DateTimeAxis.ToDouble(date);
            date = new DateTime(2018, 01, 25);
            double dateString2 = DateTimeAxis.ToDouble(date);
            date = new DateTime(2018, 01, 28);
            double dateString3 = DateTimeAxis.ToDouble(date);

            //Items = new ObservableCollection<Item>()
            //{
            //    new Item(){Label = dateString1, Value1 = 5},
            //    new Item(){Label = dateString2, Value1 = 3},
            //    new Item(){Label = dateString3, Value1 = 6},
            //};
            //timeStart();

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

            disp.BeginInvoke(new recCollection(recordChartCollection), dataCurrTemp, Temp);
            //recordChartCollection(_dataCurrTemp, Temp);
            //OnPropertyChanged("dataCurrTemp");  //уведомление View об изменении
            OnPropertyChanged("Temp");  //уведомление View об изменении
        }
        public void setViewPress()
        {
            Press = serialPortM._press; 
            disp.BeginInvoke(new recCollection(recordChartCollection), dataCurrPress, Press);
            OnPropertyChanged("Press");

        }
        public void setViewHum()
        {
            Hum = serialPortM._hum; 
            OnPropertyChanged("Hum");

        }
        public void setViewGas()
        {
            Gas = serialPortM._gas; 
            OnPropertyChanged("Gas");

        }
        //*************************************************************
        //ДИАГРАММЫ

        //Запись в коллекцию
        private void recordChartCollection(ObservableCollection<ItemCharStruct> collection, string value)
        {
            if (value != null)
            {
                string date = DateTime.Now.ToString();      //получение текущей даты

                value = value.Trim(new char[] { '\t' });    //вырезание последнего символа. Особенность получаемого символа. ИЗМЕНИТСЯ
                //value = value.Trim(new char[] { '?' });     //вырезание последнего символа. Особенность получаемого символа. ИЗМЕНИТСЯ
                value = value.Replace(".", ",");            //замена точки на запятую для парсирования в double

                double val = Double.Parse(value);           //парсирование в double
                date = date.Substring(11);                  //обрезание лишних символов. Остаётся только дата

                //collection.Add(new KeyValuePair<string, double>(date, val));    //добавление в коллекцию

                ItemCharStruct item = new ItemCharStruct() { Date = DateTimeAxis.ToDouble(DateTime.Now), Value = val };
                //Item item = new Item() { Label = DateTimeAxis.ToDouble(DateTime.Now), Value1 = val };
                collection.Add(item);
                int g = 0;
            }
        }
        //Запись в коллекцию из ФАЙЛА
        private void recordChartCollection(ObservableCollection<ItemCharStruct> collection, string date, string value)
        {
            if (value != null)
            {
                value = value.Trim(new char[] { '\t' });    //вырезание последнего символа. Особенность получаемого символа. ИЗМЕНИТСЯ
                //value = value.Trim(new char[] { '?' });     //вырезание последнего символа. Особенность получаемого символа. ИЗМЕНИТСЯ
                value = value.Replace(".", ",");            //замена точки на запятую для парсирования в double

                double val = Double.Parse(value);           //парсирование в double
                date = date.Substring(11);                  //обрезание лишних символов. Остаётся только дата

                //collection.Add(new KeyValuePair<string, double>(date, val));    //добавление в коллекцию

                ItemCharStruct item = new ItemCharStruct() { Date = DateTimeAxis.ToDouble(DateTime.Now), Value = val };
                //Item item = new Item() { Label = DateTimeAxis.ToDouble(DateTime.Now), Value1 = val };
                collection.Add(item);
                int g = 0;
            }
        }


        /// Очищение коллекции
        private void removeChartCollection(ObservableCollection<KeyValuePair<string, double>> collection)
        {
            collection.Clear(); //двойной, иначе визуально удаляется не полностью, хотя коллекция пуста
            collection.Clear();
        }


        //*********************Таймер
        //****************************
        System.Windows.Threading.DispatcherTimer timer;
        public double q { get; set; }
        public void timeStart()
        {
            timer = new System.Windows.Threading.DispatcherTimer();

            timer.Tick += new EventHandler(timerTick2);     //вызов события
            timer.Interval = new TimeSpan(0, 0, 0, 1);         //Установка интервала
            timer.Start();                                  //старт таймера
        }
        DateTime time = DateTime.Now;

        private void timerTick2(object sender, EventArgs e)
        {
            q++;
            OnPropertyChanged("q");
        }
        //**********************************


        private void readFromLog(string path, ObservableCollection<ItemCharStruct> collection)
        {
            DateTime dateFrom = DateTime.MinValue;  //дата С
            DateTime dateTo = DateTime.Now;         //дата ПО
            DateTime dateLoop;                      //дата для цикла
            int count = 0;                          //счётчик точек. Потом убрать

            //switch (cmbPeriod.Text)
            //{
            //    case "За сутки":
            //        dateFrom = DateTime.Now.AddDays(-1);
            //        break;
            //    case "За неделю":
            //        dateFrom = DateTime.Now.AddDays(-7);
            //        break;
            //    case "За месяц":
            //        dateFrom = DateTime.Now.AddDays(-31);
            //        break;
            //    case "За пол года":
            //        dateFrom = DateTime.Now.AddDays(-183);
            //        break;
            //    case "За год":
            //        dateFrom = DateTime.Now.AddDays(-366);
            //        break;
            //}

            if (collection.Count > 0)   // очистка коллекции, если в ней что-то находится
                collection.Clear();

            StreamReader streamReader = new StreamReader(path);
            while (!streamReader.EndOfStream)
            {
                string Y = streamReader.ReadLine();
                string X = streamReader.ReadLine();

                //date = X.Remove(10);
                dateLoop = DateTime.Parse(X);

                if (dateLoop >= dateFrom && dateLoop <= dateTo)
                {
                    recordChartCollection(collection, X, Y);
                    count++;                                //счётчик точек. Потом убрать
                }
            }
            streamReader.Close();

            //lblPointTemp.Content = count.ToString();        //счётчик точек. Потом убрать
        }

    }
}
