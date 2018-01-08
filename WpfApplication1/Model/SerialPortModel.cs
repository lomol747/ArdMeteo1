using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.ComponentModel;
using System.Windows.Threading;
using System.IO;

namespace ArdMeteo
{
    public class SerialPortModel
    {

        //*************************************
        //События
        public delegate void delegeteContainer();
        public event delegeteContainer eventTemp;
        public event delegeteContainer eventPress;
        //*************************************
        Dispatcher disp;

        private SerialPort serialPort = new SerialPort();   //инициализация ком порта
        public Settings settings = new Settings();         //инициализация настроек

        public string _temp;

        public string _press;   //давление

        //делегеты, используемые при получении из ком порта
        private delegate void LineReceivedEventTemp(string temp);   
        private delegate void LineReceivedEventPress(string press);

        public SerialPortModel()
        {
            disp = Dispatcher.CurrentDispatcher;
            InitializeSerial();
        }


        public void InitializeSerial () //подключение к ком порту
        {
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

                //lblPort.Foreground = Brushes.Green;    //текущий ком порт внизу окна

                //tt++;
                //lblTest.Content = tt.ToString();
            }
            catch
            {
                //lblPort.Foreground = Brushes.Red;      //текущий копм порт внизу окна

                //MessageBox.Show("Отсутствует подключение к устройству", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);  //исключение для ошибки
            }
        }

        private string setValue(string val)
        {
            return val; ;
        }

        private void SerialPort1_DataReceivedTemp(object sender, SerialDataReceivedEventArgs e) //получение температуры
        {
            //Dispatcher disp = new Dispatcher(dwwdw);
            bool g = true;
            
                if (serialPort.IsOpen)
                {
                    string test = Dispatcher.CurrentDispatcher.ToString();
                    _temp = serialPort.ReadLine();  //получение температуры
                    eventTemp();                    //рассылка события
                    disp.BeginInvoke(new LineReceivedEventTemp(LineReceivedTemp), _temp);   //вызов в потоке записи в файл
                                                   //Dispatcher.CurrentDispatcher.BeginInvoke(new LineReceivedEventTemp(LineReceivedTemp), _temp);   //вызов в потоке записи в файл
                int dssd = 0;

                }
            try
            {
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

                _press = serialPort.ReadLine();   //получаем строку. Должна быть влажность
                eventPress();

                //Dispatcher.CurrentDispatcher.BeginInvoke(new LineReceivedEventPress(LineReceivedPress), _press);    //выполнение делегата

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
            //lblPing.Foreground = Brushes.Red;   //пинг
            //lblPing.Background = Brushes.Red;
            //fPing = true;                       //флаг для таймера

            //tempBox.Text = temp;
            string path = "Лог температуры.txt";            //наименование файла с логами
            string date = DateTime.Now.ToString();

            recLog(path, date, temp);
            //recordChartCollection(dataCurrTemp, date, temp);

        }

        private void LineReceivedPress(string pressure)
        {
            //pressBox.Text = pressure;                       //отображение значения
            string path = "Лог атмосферного давления.txt";   //Наименование файла с логами
            string date = DateTime.Now.ToString();          //получаем текущую дату

            recLog(path, date, pressure);
            //recordChartCollection(dataCurrPress, date, pressure);
        }
        //***********************************

        /// Построчная запись в лог
        private void recLog(string path, string date, string val)
        {
            using (StreamWriter sw = File.AppendText(path)) //директива для записи в файл
            {
                sw.WriteLine(val);     //запись значения
                sw.WriteLine(date);    //запись времени
            }
        }

    }
}
