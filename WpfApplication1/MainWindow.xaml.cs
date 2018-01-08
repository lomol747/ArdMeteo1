using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Drawing;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO.Ports;
using System.IO;
using System.Windows.Controls.DataVisualization.Charting;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace ArdMeteo
{
    public partial class MainWindow : Window
    {
        //********************************************
        //Глобальные переменные
        //SerialPort serialPort = new SerialPort();
        //Settings settings = new Settings();

        //private bool fPing = false;     // флаг-пинг

        //private delegate void LineReceivedEventTemp(string temp);
        //private delegate void LineReceivedEventPress(string press);

        //Chart.SampleModel dataSample;

        ////Таймер
        System.Windows.Threading.DispatcherTimer timer;
        //int s;
        ////
        //struct logStruct { string date, val;  }

        ////Коллекции для графиков
        //ObservableCollection<KeyValuePair<string, double>> dataCurrTemp     = new ObservableCollection<KeyValuePair<string, double>>();    //коллекция текущей температруы
        //ObservableCollection<KeyValuePair<string, double>> dataCurrPress    = new ObservableCollection<KeyValuePair<string, double>>();    //коллекция текущего давления
        //ObservableCollection<KeyValuePair<string, double>> dataTemp         = new ObservableCollection<KeyValuePair<string, double>>();    //коллекция температуры
        //ObservableCollection<KeyValuePair<string, double>> dataPress        = new ObservableCollection<KeyValuePair<string, double>>();    //коллекция давления

        ObservableCollection<KeyValuePair<string, double>> dataTest         = new ObservableCollection<KeyValuePair<string, double>>();    //коллекция тестовая

        //int tt = 0;

        //********************************************
        public MainWindow()
        {
            //serialPort = new SerialPort();
            //settings = new Settings();

            InitializeComponent();
            //initSerialPort();

            //dataSample = new Chart.SampleModel();

            //chartCurrPress.DataContext = dataSample;
            
            //showColumnChart();
            double test = chartCurrTemp.Width;
            //cmbPeriod.SelectedIndex = getItemIndex(cmbPeriod.Items, "За сутки");
            init();
            chartTest.DataContext = dataTest;         //тестовое
            

            string test5 = Dispatcher.CurrentDispatcher.ToString();
            string test6 = this.Dispatcher.ToString();
            int fd = 0;
        }




        public void init()
        {
            dataTest.Add(new KeyValuePair<string, double>("Dog", 30));
            dataTest.Add(new KeyValuePair<string, double>("Cat", 25));
            dataTest.Add(new KeyValuePair<string, double>("Rat", 5));
            dataTest.Add(new KeyValuePair<string, double>("Hampster", 8));
            dataTest.Add(new KeyValuePair<string, double>("Rabbit", 12));

            //OnPropertyChanged("dataCurrTemp");
        }

        //    private void showColumnChart()  //метод для  подключения коллекций к графикам 
        //    {
        //        chartCurrTemp.DataContext   = dataCurrTemp;     //текущая температура
        //        chartCurrPress.DataContext  = dataCurrPress;    //текущее давление
        //        chartTemp.DataContext       = dataTemp;         //температура
        //        chartPress.DataContext      = dataPress;        //давление

        //        chartTest.DataContext       = dataTest;         //тестовое

        //        init();
        //    }

        //    //*************************************************
        //    //поток ком порта
        //    //*************************************************
        //    private void SerialPort1_DataReceivedTemp(object sender, SerialDataReceivedEventArgs e)
        //    {
        //        try
        //        {
        //            if (serialPort.IsOpen)
        //            {
        //                string temp = serialPort.ReadLine();
        //                this.Dispatcher.BeginInvoke(new LineReceivedEventTemp(LineReceivedTemp), temp);


        //            }
        //        }
        //        catch   //КОСТЫЛЬ!!! Будет выпадать ошибка, не выяснено почему, скорее всего что-то со строками и буфером. Не критично, код дальше продолжает выполняться
        //        {
        //            //MessageBox.Show("Ошибка получения температуры", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);  //исключение для ошибки
        //        }
        //    }

        //    private void SerialPort1_DataReceivedPress(object sender, SerialDataReceivedEventArgs e)
        //    {
        //        try
        //        {
        //            string pressure = serialPort.ReadLine();   //получаем строку. Должна быть влажность

        //            this.Dispatcher.BeginInvoke(new LineReceivedEventPress(LineReceivedPress), pressure);    //выполнение делегата
        //        }
        //        catch   //КОСТЫЛЬ!!! Будет выпадать ошибка, не выяснено почему, скорее всего что-то со строками и буфером. Не критично, код дальше продолжает выполняться
        //        {
        //            //MessageBox.Show("Ошибка получения давления", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);  //исключение для ошибки
        //        }
        //    }

        //    //*********************************
        //    //Запись в файл
        //    private void LineReceivedTemp(string temp)
        //    {
        //        lblPing.Foreground = Brushes.Red;   //пинг
        //        lblPing.Background = Brushes.Red;
        //        fPing = true;                       //флаг для таймера

        //        tempBox.Text = temp;
        //        string path = "Лог температуры.txt";            //наименование файла с логами
        //        string date = DateTime.Now.ToString();

        //        recLog(path, date, temp);
        //        recordChartCollection(dataCurrTemp, date, temp);

        //    }

        //    private void LineReceivedPress(string pressure)
        //    {
        //        pressBox.Text = pressure;                       //отображение значения
        //        string path = "Лог атмосферного давления.txt";   //Наименование файла с логами
        //        string date = DateTime.Now.ToString();          //получаем текущую дату

        //        recLog(path, date, pressure);
        //        recordChartCollection(dataCurrPress, date, pressure);

        //    }


        //    //**********************************
        //    ////////Инициализация ком-порта
        //    private void initSerialPort()
        //    {
        //        lblPort.Content = settings.getPortName();    //выводим текущий ком порт

        //        try
        //        {
        //            if (serialPort.IsOpen) //если ранее порт был открыт, то закрываем. Используется для изменения настроек
        //            {
        //                serialPort.DiscardInBuffer();  //их наличие под вопросом
        //                serialPort.DiscardOutBuffer(); //их наличие под вопросом

        //                serialPort.Close();

        //            }

        //            serialPort.PortName = settings.getPortName();  //имя порта
        //            serialPort.BaudRate = settings.getBaudRate();  //скорость в бодах
        //            serialPort.DtrEnable = true;                   //готовность для обмена данными
        //            serialPort.Open();                             //открываем последовательное соединение

        //            serialPort.DataReceived += SerialPort1_DataReceivedTemp;
        //            serialPort.DataReceived += SerialPort1_DataReceivedPress;

        //            lblPort.Foreground = Brushes.Green;    //текущий ком порт внизу окна

        //            tt++;
        //            lblTest.Content = tt.ToString();
        //        }
        //        catch
        //        {
        //            lblPort.Foreground = Brushes.Red;      //текущий копм порт внизу окна

        //            MessageBox.Show("Отсутствует подключение к устройству", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);  //исключение для ошибки
        //        }
        //    }

        //    //******************************************************************************************
        //Теймер
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            timer = new System.Windows.Threading.DispatcherTimer();

            timer.Tick += new EventHandler(timerTick2);     //вызов события
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);         //Установка интервала
            double test = chartCurrTemp.Width;
            timer.Start();                                  //старт таймера
        }

        private void timerTick2(object sender, EventArgs e)
        {
            double test = chartCurrTemp.Width;
        }

        //    private void timerTick2(object sender, EventArgs e) //вызываемая функция таймера
        //    {
        //        if (fPing)
        //        {
        //            if (s <= 5)
        //            {
        //                s++;
        //            }
        //            else
        //            {
        //                fPing = false;
        //                s = 0;
        //                lblPing.Foreground = Brushes.Black;
        //                lblPing.Background = Brushes.Black;
        //            }
        //        }
        //    }

        //    public class Test
        //    {
        //        //public string Image { get; set; }
        //        public string Text { get; set; }
        //    }


        //    //***************************************************************************
        //    //Обработка нажатий кнопок
        //    //***************************************************************************

        //    private void button5_Click(object sender, RoutedEventArgs e)
        //    {

        //        //cmbPeriod.SelectedIndex = cmbPeriod.Items.IndexOf("За сутки");
        //        readFromLog("Лог температуры.txt", dataTemp);

        //    }



        //    private void button6_Click(object sender, RoutedEventArgs e)
        //    {
        //        removeChartCollection(dataTemp);
        //    }



        //    //********************************************************************************************
        //    //ДИАГРАММЫ

        //    /// <summary>
        //    /// Запись в коллекции связанные с графиками
        //    /// </summary>
        //    /// <param name="collection">Коллекция</param>
        //    /// <param name="date">Дата</param>
        //    /// <param name="value">Значение</param>
        //    private void recordChartCollection(ObservableCollection<KeyValuePair<string, double>> collection, string date, string value)
        //    {


        //        value = value.Trim(new char[] { '\t' });
        //        value = value.Replace(".", ",");

        //        double val = Double.Parse(value);
        //        date = date.Substring(11);

        //        collection.Add(new KeyValuePair<string, double>(date, val));
        //    }

        //    /// <summary>
        //    /// Очищение коллекции
        //    /// </summary>
        //    /// <param name="collection">коллекция</param>
        //    private void removeChartCollection(ObservableCollection<KeyValuePair<string, double>> collection)
        //    {
        //        collection.Clear(); //двойной, иначе визуально удаляется не полностью, хотя коллекция пуста
        //        collection.Clear();
        //    }


        //    //********************************************************************************
        //    /// <summary>
        //    /// Построчная запись в лог
        //    /// </summary>
        //    /// <param name="path">Наименование лога</param>
        //    /// <param name="date">дата</param>
        //    /// <param name="val">значение</param>
        //    private void recLog(string path, string date, string val)
        //    {
        //        using (StreamWriter sw = File.AppendText(path)) //директива для записи в файл
        //        {
        //            sw.WriteLine(val);     //запись значения
        //            sw.WriteLine(date);    //запись времени
        //        }
        //    }

        //    /// <summary>
        //    /// Чтение из файла и запись в график
        //    /// </summary>
        //    /// <param name="path">Наименование файла</param>
        //    /// <param name="collection">Коллекция</param>
        //    private void readFromLog(string path, ObservableCollection<KeyValuePair<string, double>> collection)
        //    {
        //        DateTime dateFrom = DateTime.MinValue;  //дата С
        //        DateTime dateTo = DateTime.Now;         //дата ПО
        //        DateTime dateLoop;                      //дата для цикла
        //        int count = 0;                          //счётчик точек. Потом убрать

        //        switch (cmbPeriod.Text)
        //        {
        //            case "За сутки":
        //                dateFrom = DateTime.Now.AddDays(-1);
        //                break;
        //            case "За неделю":
        //                dateFrom = DateTime.Now.AddDays(-7);
        //                break;
        //            case "За месяц":
        //                dateFrom = DateTime.Now.AddDays(-31);
        //                break;
        //            case "За пол года":
        //                dateFrom = DateTime.Now.AddDays(-183);
        //                break;
        //            case "За год":
        //                dateFrom = DateTime.Now.AddDays(-366);
        //                break;
        //        }

        //        if (dataTemp.Count > 0)
        //            dataTemp.Clear();

        //        StreamReader streamReader = new StreamReader(path);
        //        while (!streamReader.EndOfStream)
        //        {
        //            string Y = streamReader.ReadLine();
        //            string X = streamReader.ReadLine();

        //            //date = X.Remove(10);
        //            dateLoop = DateTime.Parse(X);

        //            if (dateLoop >= dateFrom && dateLoop <= dateTo)
        //            {
        //                recordChartCollection(collection, X, Y);
        //                count++;                                //счётчик точек. Потом убрать
        //            }
        //        }
        //        streamReader.Close();

        //        lblPointTemp.Content = count.ToString();        //счётчик точек. Потом убрать
        //    }

        //    //********************************************************************************
        //    //Вспомогательные функции

        //    /// <summary>
        //    /// Поиск элемента в ItemCollection
        //    /// </summary>
        //    /// <param name="item">ItemCollection</param>
        //    /// <param name="val">Искомое значение</param>
        //    /// <returns></returns>
        //    private int getItemIndex(ItemCollection item, string val)   //костылятина.
        //    {
        //        string name;

        //        for (int i = 0; 0 < item.Count; i++)
        //        {
        //            name = item[i].ToString();  //получаем значение
        //            name = name.Substring(38);  //вырезаем  первую ненужную часть
        //            if (val.Equals(name))
        //                return i;
        //        }
        //        return -1;
        //    }

        //    private void cmbPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //    {
        //        string test = (sender as ComboBox).Text;
        //        object send = sender;


        //        //removeChartCollection(dataTemp);
        //        //readFromLog("Лог температуры.txt", dataTemp);

        //    }

        //    private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        //    {
        //        removeChartCollection(dataTemp);
        //        readFromLog("Лог температуры.txt", dataTemp);
        //    }









        //    //private void startChart(Chart chart, string filePath)  //графики на второй вкладке
        //    //{

        //    //DateTime dateFrom = DateTime.MinValue;  //дата С
        //    //DateTime dateTo = DateTime.Now;         //дата ПО
        //    //DateTime dateLoop;                      //дата для цикла

        //    //switch (cmbPeriod.SelectedItem.ToString())
        //    //{
        //    //    case "За сутки":
        //    //        dateFrom = DateTime.Now.AddDays(-1);
        //    //        break;
        //    //    case "За неделю":
        //    //        dateFrom = DateTime.Now.AddDays(-7);
        //    //        break;
        //    //    case "За месяц":
        //    //        dateFrom = DateTime.Now.AddDays(-31);
        //    //        break;
        //    //    case "За пол года":
        //    //        dateFrom = DateTime.Now.AddDays(-183);
        //    //        break;
        //    //    case "За год":
        //    //        dateFrom = DateTime.Now.AddDays(-366);
        //    //        break;
        //    //}


        //    ////chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0, 200);
        //    //chart.ChartAreas[0].CursorX.IsUserEnabled = true;
        //    //chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
        //    //chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
        //    //chart.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
        //    ////chart1.ChartAreas[0].AxisY.Interval = 2;
        //    ////chart.Series[0].Color = Color.Red;
        //    //chart.Series[0].BorderWidth = 1;

        //    //chart.Series[0].Points.Clear();


        //    //StreamReader streamReader = new StreamReader(filePath);
        //    //while (!streamReader.EndOfStream)
        //    //{
        //    //    string Y = streamReader.ReadLine();
        //    //    string X = streamReader.ReadLine();

        //    //    //date = X.Remove(10);
        //    //    dateLoop = DateTime.Parse(X);


        //    //    if (dateLoop >= dateFrom && dateLoop <= dateTo)
        //    //    {

        //    //        chart.Series[0].Points.AddXY(X, Y);
        //    //    }
        //    //}
        //    //streamReader.Close();

        //    ////scaleChart(chart);

        //    ////костыль для отображения колиества точек
        //    //if (chart.Equals(chart1))
        //    //    countPointTemp.Text = "Количество точек: " + chart.Series[0].Points.Count();
        //    //else if (chart.Equals(chart3))
        //    //    countPointPressure.Text = "Количество точек: " + chart.Series[0].Points.Count();

        //    //fChart = true;

        //    //}
    }
}
