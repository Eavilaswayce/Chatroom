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

namespace ChatRoom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        public void Connect(string server, string message)
        {
            try
            {
                // Create a TcpClient.
                Int32 port = Int32.Parse(portTextBox.Text);
                TcpClient client = new TcpClient(server, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);
                Debug.WriteLine($"Sent: {message}");

                // Buffer to store the response bytes.
                data = new byte[256];

                // String to store the response ASCII representation.
                string responseData = string.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Debug.WriteLine($"Recieved: {responseData}");
                bigBox.AppendText($"{username.Text}: {message}{Environment.NewLine}");

                // Close Everything
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Debug.WriteLine($"ArgumentNullException: {e}");
            }
            catch (SocketException e)
            {
                Debug.WriteLine($"SocketException: {e}");
            }
        }

        public void Listener()
        {
            TcpListener server = null;

            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = Int32.Parse("13000");
                IPAddress localAddr = IPAddress.Parse("192.168.0.11");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                while (true)
                {
                    Debug.WriteLine("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Debug.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Debug.WriteLine($"Received: {data}");

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Debug.WriteLine($"Sent: {data}");
                        //bigBox.AppendText($"{data}{Environment.NewLine}");
                        MessageBox.Show("New message: " + data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Debug.WriteLine($"SocketException: {e}");
            }
            finally
            {
                server.Stop();
            }
        }

        private void chatBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Connect(ipTextBox.Text, chatBox.Text);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new Thread(delegate () {
                Listener();
            }).Start();
        }
    }
}