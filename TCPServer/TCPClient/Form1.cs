using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TCPClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string srcPath;
        public string initPath;
        private const int BufferSize = 1024;
        public Thread TU = null;
        public string FileNameExtension;

        private void button1_Click(object sender, EventArgs e)
        {
            initPath = Environment.CurrentDirectory;
            openFileDialog1.InitialDirectory = initPath;
            openFileDialog1.FileName = "*.*";
            openFileDialog1.Filter = "All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (openFileDialog1.OpenFile() != null)
                    {
                        srcPath = openFileDialog1.FileName;
                        FileNameExtension = System.IO.Path.GetFileName(srcPath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Файл не может быть прочитан" + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (srcPath != null)
            {
                SendTCP(srcPath, textBox1.Text, Convert.ToInt16(textBox2.Text));
            }
            else
            {
                MessageBox.Show("Выберите файл");
            }
        }

        public void SendTCP(string M, string IPA, Int32 PortN)
        {
            byte[] SendingBuffer = null;
            TcpClient client = null;
            NetworkStream netstream = null;
            int fileNameLength = FileNameExtension.Length;
            try
            {
                client = new TcpClient(IPA, PortN);
                label3.Text = "Соединение установлено\n";
                netstream = client.GetStream();
                FileStream Fs = new FileStream(M, FileMode.Open, FileAccess.Read);
                int NoOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Fs.Length) / Convert.ToDouble(BufferSize))) + 1;
                progressBar1.Maximum = NoOfPackets;
                progressBar1.Value = progressBar1.Minimum;
                progressBar1.Step = 1;
                int TotalLength = (int)Fs.Length, CurrentPacketLength, counter = 0;
                byte[] nameArray = new byte[1024];
                nameArray = ASCIIEncoding.Default.GetBytes(FileNameExtension);
                Stream StrS = new MemoryStream(nameArray);
                SendingBuffer = new byte[1024];
                StrS.Read(SendingBuffer, 0, 1024);
                netstream.Write(SendingBuffer, 0, (int)SendingBuffer.Length);
                StrS.Close();
                for (int i = 0; i < NoOfPackets;i++ )
                {
                    if (TotalLength > BufferSize)
                    {
                        CurrentPacketLength = BufferSize;
                        TotalLength = TotalLength - CurrentPacketLength;
                    }
                    else
                    {
                        CurrentPacketLength = TotalLength;
                    }
                    SendingBuffer = new byte[CurrentPacketLength];
                    Fs.Read(SendingBuffer, 0, CurrentPacketLength);
                    netstream.Write(SendingBuffer, 0, (int)SendingBuffer.Length);
                    progressBar1.PerformStep();
                    if (progressBar1.Value == progressBar1.Maximum)
                    {
                        MessageBox.Show("Файл передан успешно");
                    }
                }
                Fs.Close();
                netstream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
