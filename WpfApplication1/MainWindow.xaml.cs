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
        SerialPort serialPort = new SerialPort();
        Settings settings = new Settings();

        private bool fPing = false;     // флаг-пинг

        private delegate void LineReceivedEventTemp(string temp);
        private delegate void LineReceivedEventPress(string press);

        Chart.SampleModel dataSample;

        //Таймер
        System.Windows.Threading.DispatcherTimer timer;
        int s;
        //

        //********************************************
        ObservableCollection<KeyValuePair<string, double>> Power = new ObservableCollection<KeyValuePair<string, double>>();


        public MainWindow()
        {
            //serialPort = new SerialPort();
            //settings = new Settings();

            InitializeComponent();
            initSerialPort();

            dataSample = new Chart.SampleModel();

            chartCurrPress.DataContext = dataSample;
            int fd = 4;


            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);  // per 5 seconds, you could change it
            timer.Tick += new EventHandler(timer_Tick);
            timer.IsEnabled = true;

            showColumnChart();
        }


        double i = 5;
        Random random = new Random(DateTime.Now.Millisecond);
        void timer_Tick(object sender, EventArgs e)
        {
            //Power.Add(new KeyValuePair<double, double>(i, random.NextDouble()));
            i += 5;
        }

        private void showColumnChart()
        {
            chartTest.DataContext = Power;
        }

        //*************************************************
        //поток ком порта
        //*************************************************
        private void SerialPort1_DataReceivedTemp(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    string temp = serialPort.ReadLine();
                    this.Dispatcher.BeginInvoke(new LineReceivedEventTemp(LineReceivedTemp), temp);


                }
            }
            catch   //КОСТЫЛЬ!!! Будет выпадать ошибка, не выяснено почему, скорее всего что-то со строками и буфером. Не критично, код дальше продолжает выполняться
            {
                //MessageBox.Show("Ошибка получения температуры", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);  //исключение для ошибки
            }
        }

        private void SerialPort1_DataReceivedPress(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string pressure = serialPort.ReadLine();   //получаем строку. Должна быть влажность

                this.Dispatcher.BeginInvoke(new LineReceivedEventPress(LineReceivedPress), pressure);    //выполнение делегата
            }
            catch   //КОСТЫЛЬ!!! Будет выпадать ошибка, не выяснено почему, скорее всего что-то со строками и буфером. Не критично, код дальше продолжает выполняться
            {
                //MessageBox.Show("Ошибка получения давления", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);  //исключение для ошибки
            }
        }

        //*********************************
        //Запись в файл
        private void LineReceivedTemp(string temp)
        {
            lblPing.Foreground = Brushes.Red;   //пинг
            lblPing.Background = Brushes.Red;
            fPing = true;                       //флаг для таймера

            tempBox.Text = temp;
            string path = "Лог температуры.txt";            //наименование файла с логами
            string date = DateTime.Now.ToString();
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(temp);
                sw.WriteLine(date);
            }

            temp = temp.Trim(new char[] { '\t' });
            temp = temp.Replace(".", ",");
            date = date.Substring(11);
            double tempD = Double.Parse(temp);

            Power.Add(new KeyValuePair<string, double>(date, tempD));

        }

        private void LineReceivedPress(string pressure)
        {
            pressBox.Text = pressure;                       //отображение значения
            string path = "Лог атмосферного давления.txt";   //Наименование файла с логами
            string date = DateTime.Now.ToString();          //получаем текущую дату
            using (StreamWriter sw = File.AppendText(path)) //директива для записи в файл
            {
                sw.WriteLine(pressure);     //запись давления
                sw.WriteLine(date);         //запись времени
            }
        }


        //**********************************
        ////////Инициализация ком-порта
        private void initSerialPort()
        {
            lblPort.Content = settings.getPortName();    //выводим текущий ком порт

            try
            {
                if (serialPort.IsOpen) //если ранее порт был открыт, то закрываем. Используется для изменения настроек
                {
                    serialPort.DiscardInBuffer();  //их наличие под вопросом
                    serialPort.DiscardOutBuffer(); //их наличие под вопросом

                    serialPort.Close();

                }

                serialPort.PortName = settings.getPortName();  //имя порта
                serialPort.BaudRate = settings.getBaudRate();  //скорость в бодах
                serialPort.DtrEnable = true;                   //готовность для обмена данными
                serialPort.Open();                             //открываем последовательное соединение

                serialPort.DataReceived += SerialPort1_DataReceivedTemp;
                serialPort.DataReceived += SerialPort1_DataReceivedPress;

                lblPort.Foreground = Brushes.Green;    //текущий ком порт внизу окна
            }
            catch
            {
                lblPort.Foreground = Brushes.Red;      //текущий копм порт внизу окна

                MessageBox.Show("Отсутствует подключение к устройству", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);  //исключение для ошибки
            }
        }

        //******************************************************************************************
        //Теймер
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            timer = new System.Windows.Threading.DispatcherTimer();

            timer.Tick += new EventHandler(timerTick2);     //вызов события
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);         //Установка интервала
            timer.Start();                                  //старт таймера
        }

        private void timerTick2(object sender, EventArgs e) //вызываемая функция таймера
        {
            if (fPing)
            {
                if (s <= 5)
                {
                    s++;
                }
                else
                {
                    fPing = false;
                    s = 0;
                    lblPing.Foreground = Brushes.Black;
                    lblPing.Background = Brushes.Black;
                }
            }
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {

        }
        //********************************************************************************************
        //ДИАГРАММЫ
        //private void startChart(Chart chart, string filePath)  //графики на второй вкладке
        //{

        //DateTime dateFrom = DateTime.MinValue;  //дата С
        //DateTime dateTo = DateTime.Now;         //дата ПО
        //DateTime dateLoop;                      //дата для цикла

        //switch (cmbPeriod.SelectedItem.ToString())
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


        ////chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0, 200);
        //chart.ChartAreas[0].CursorX.IsUserEnabled = true;
        //chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
        //chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
        //chart.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
        ////chart1.ChartAreas[0].AxisY.Interval = 2;
        ////chart.Series[0].Color = Color.Red;
        //chart.Series[0].BorderWidth = 1;

        //chart.Series[0].Points.Clear();


        //StreamReader streamReader = new StreamReader(filePath);
        //while (!streamReader.EndOfStream)
        //{
        //    string Y = streamReader.ReadLine();
        //    string X = streamReader.ReadLine();

        //    //date = X.Remove(10);
        //    dateLoop = DateTime.Parse(X);


        //    if (dateLoop >= dateFrom && dateLoop <= dateTo)
        //    {

        //        chart.Series[0].Points.AddXY(X, Y);
        //    }
        //}
        //streamReader.Close();

        ////scaleChart(chart);

        ////костыль для отображения колиества точек
        //if (chart.Equals(chart1))
        //    countPointTemp.Text = "Количество точек: " + chart.Series[0].Points.Count();
        //else if (chart.Equals(chart3))
        //    countPointPressure.Text = "Количество точек: " + chart.Series[0].Points.Count();

        //fChart = true;

        //}
    }
}
