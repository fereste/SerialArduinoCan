using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace ArduinoController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SerialPort port = new SerialPort("COM5", 115200);
        private Thread scanThread;

        public MainWindow()
        {
            port.Open();
            InitializeComponent();
            new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    try
                    {
                        Debug.WriteLine(port.ReadLine());
                    }
                    catch
                    {

                    }
                }
            })).Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //port.Open();
            if (slider0 == null || slider1 == null || slider2 == null || slider3 == null ||
                slider4 == null || slider5 == null || slider6 == null || slider7 == null)
                return;

            int id = int.Parse(txtId.Text, System.Globalization.NumberStyles.HexNumber);

            byte[] buff = new byte[]
            {
                (byte)((id & 0xF00) >> 8),
                (byte)(id & 0x0FF),
                (byte)slider0.Value,
                (byte)slider1.Value,
                (byte)slider2.Value,
                (byte)slider3.Value,
                (byte)slider4.Value,
                (byte)slider5.Value,
                (byte)slider6.Value,
                (byte)slider7.Value
            };
            //buff = buff.Reverse().ToArray();
            port.Write(buff, 0, buff.Length);
            //port.Close();
        }

        private void Scan_Click(object sender, RoutedEventArgs e)
        {
            scanThread = new Thread(new ThreadStart(Scan));
            scanThread.Start();
        }

        private void Scan()
        {
            byte[] buff = new byte[] { 0, 0, 127, 127, 127, 127, 127, 127, 127, 127 };
            for (int i = 0x300; i < 0xFFF; i++)
            {
                buff[0] = (byte)((i & 0xF00) >> 8);
                buff[1] = (byte)(i & 0x0FF);

                Debug.WriteLine(string.Format("0x{0:X}", i));
                port.Write(buff, 0, buff.Length);
                Thread.Sleep(500);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (scanThread != null && scanThread.IsAlive)
                scanThread.Abort();

            if (port.IsOpen)
                port.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string data = string.Format("{0}|{1}|\0", txtRpm.Text, txtSpeed.Text);
            port.Write(data);
        }
    }
}
