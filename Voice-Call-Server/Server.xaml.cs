using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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

namespace Voice_Call_Server
{
    public partial class MainWindow : Window
    {
        const int PORT = 4321;
        TcpListener listener = null;
        BackgroundWorker serverBackgroundWorker = null;
        WaveOut waveOut = null;

        int index = 0;

        byte[] firstBytes;
        byte[] secondBytes;
        public MainWindow()
        {
            InitializeComponent();
            serverBackgroundWorker = new BackgroundWorker();
            serverBackgroundWorker.DoWork += ServerBackgroundWorker_DoWork;
            serverBackgroundWorker.RunWorkerAsync();

            waveOut = new WaveOut();
        }

        private void ServerBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            listener = new TcpListener(IPAddress.Any, PORT);
            listener.Start();

            TcpClient tcp = listener.AcceptTcpClient();
            NetworkStream stream = tcp.GetStream();
            while (true)
            {
                int len = 0;
                len += stream.ReadByte() * 256 * 256;
                len += stream.ReadByte() * 256;
                len += stream.ReadByte();

                IWaveProvider provider;
                if (index == 0)
                {
                    firstBytes = new byte[len];
                    stream.Read(firstBytes, 0, len);
                    if(secondBytes == null)
                    {
                        index = 1;
                        continue;
                    }
                    provider = new RawSourceWaveStream(
                         new MemoryStream(secondBytes), new WaveFormat()
                    );
                    index = 1;
                }
                else
                {
                    secondBytes = new byte[len];
                    stream.Read(secondBytes, 0, len);
                    provider = new RawSourceWaveStream(
                         new MemoryStream(firstBytes), new WaveFormat()
                    );
                    index = 0;
                }
                waveOut.Init(provider);
                waveOut.Play();
            }
        }
    }
}
