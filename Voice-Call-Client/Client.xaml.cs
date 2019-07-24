using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Wave;
namespace Voice_Call_Client
{
    public partial class MainWindow : Window
    {
        const string IP = "127.0.0.1";
        const int PORT = 4321;

        WaveIn waveSource = null;
        

        TcpClient tcpClient = null;
        NetworkStream stream = null;
        List<byte> vs = new List<byte>();
        public MainWindow()
        {
            InitializeComponent();

            waveSource = new WaveIn();
            waveSource.WaveFormat = new WaveFormat(48000, 2);
            waveSource.RecordingStopped += WaveSource_RecordingStopped;
            waveSource.DataAvailable += WaveSource_DataAvailable;
            waveSource.StartRecording();

            tcpClient = new TcpClient();
            tcpClient.Connect(IP, PORT);

            stream = tcpClient.GetStream();



        }

        private void ClientBackgroundWokrer_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        private void WaveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }
        }

        private void WaveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            int len = e.BytesRecorded;
            stream.WriteByte(  (byte)(len / 256 / 256)  );
            stream.WriteByte(  (byte)(len / 256)  );
            stream.WriteByte(  (byte)(len % 256)  );

            stream.Write(e.Buffer, 0, e.BytesRecorded);
        }
    }
}
