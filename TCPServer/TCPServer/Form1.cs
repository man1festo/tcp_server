using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace TCPServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private const int BufferSize = 1024;
        public string Status = string.Empty;
        public Thread T = null;
        private string FileNameExtension;

        public void StartReceiving()
        {
            ReceiveTCP(Convert.ToInt16(textBox1.Text));
        }

        public void ReceiveTCP(int portN)
        {
            TcpListener Listener = null;
            try
            {
                Listener = new TcpListener(IPAddress.Any, portN);
                Listener.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            byte[] RecData = new byte[BufferSize];
            int RecBytes;

            for (; ; )
            {
                TcpClient client = null;
                NetworkStream netstream = null;
                Status = string.Empty;
                try
                {
                    string message = "Принять файл?";
                    string caption = "Вхоядщее соединение";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result;

                    if (Listener.Pending())
                    {
                        client = Listener.AcceptTcpClient();
                        netstream = client.GetStream();
                        Status = "Клиент подключен\n";
                        result = MessageBox.Show(message, caption, buttons);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            int totalrecbytes = 0;
                            FileStream Fs = new FileStream("our_file.txt", FileMode.Create, FileAccess.Write);

                            int aa1 = 0;
                            netstream.Read(RecData, 0, RecData.Length);
                            for (aa1 = 0; RecData[aa1] != 0; aa1++)
                            { }

                         
                            Fs.Write(RecData, 0, aa1);
                            Fs.Close();
                            FileNameExtension = System.IO.File.ReadAllText("our_file.txt", Encoding.Default);

                            FileStream Fs1 = new FileStream(FileNameExtension, FileMode.Create, FileAccess.Write);
                            while ((RecBytes = netstream.Read(RecData, 0, RecData.Length)) > 0)
                            {
                                Fs1.Write(RecData, 0, RecBytes);
                                totalrecbytes += RecBytes;
                            }
                            Fs1.Close();
                            netstream.Close();
                            client.Close();
                        }
                    }
                    
                }

                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                label1.Text = "Сервер запущен";
                ThreadStart Ts = new ThreadStart(StartReceiving);
                T = new Thread(Ts);
                T.Start();
            }
            else
            {
                MessageBox.Show("Вы забыли ввести порт");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (label1.Text == "Сервер запущен")
            {
                T.Abort();
                label1.Text = "Сервер отключен";
            }
        }
    }
}
