using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace ChatRoom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Socket client;
        public string confirmation;

        public MainWindow()
        {
            InitializeComponent();
        }

        // State object for receiving data from remote device.  
        public class StateObject
        {
            // Client socket.  
            public Socket workSocket = null;
            // Size of receive buffer.  
            public const int BufferSize = 256;
            // Receive buffer.  
            public byte[] buffer = new byte[BufferSize];
            // Received data string.  
            public StringBuilder sb = new StringBuilder();
        }

        // The port number for the remote device.  
        private const int port = 53;

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        // The response from the remote device.  
        private static String response = String.Empty;

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                confirmation = $"Socket connected to {client.RemoteEndPoint.ToString()}";

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        public void ReConnect()
        {
            try
            {
                // Establish the remote endpoint for the socket.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(ipTextBox.Text);
                IPAddress ipAddress = ipHostInfo.AddressList[0];

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();
            }
            catch (Exception ex)
            {
                bigBox.AppendText(ex.Message);
            }
        }

        public void Connect()
        {
            try
            {
                // Establish the remote endpoint for the socket.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(ipTextBox.Text);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                bigBox.AppendText($"{Environment.NewLine}Connecting to IP address: {ipAddress} on port: {port}");

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();
                bigBox.AppendText($"{Environment.NewLine}{confirmation}");
            }
            catch (Exception ex)
            {
                bigBox.AppendText(ex.Message);
            }
        }

        public void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            try
            {
                client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
                ReConnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "ChatRoom", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Debug.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            Connect();

            Thread thread = new Thread(check);
            thread.Start();
        }

        public void check()
        {
            while (true)
            {
                WebClient script = new WebClient();
                string messages = script.DownloadString("http://82.9.208.217:8080/");
                this.Dispatcher.Invoke(() =>
                {
                    bigBox.Document.Blocks.Clear();
                    bigBox.Document.Blocks.Add(new Paragraph(new Run(messages)));
                    //Debug.WriteLine(messages);
                });
            }
        }

        private void disconnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                bigBox.AppendText($"{Environment.NewLine}Disonnected.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"You are not connected to the ChatRoom.", "ChatRoom", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void chatBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Send(client, chatBox.Text);
                chatBox.Text = "";
            }
        }
    }
}