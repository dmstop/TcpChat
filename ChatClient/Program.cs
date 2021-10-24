using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatClient
{
    class Program
    {
        private static NetworkStream _stream;

        private static string _userName;
        private static string _host = "localhost";
        private static int _port = 1337;
        private static TcpClient _client;
        
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            if (args != null && args.Length > 1)
            {
                try
                {
                    _host = args[0];
                    _port = Convert.ToInt32(args[1]);
                }
                catch (Exception)
                {
                    Console.WriteLine("Введи \"ChatClient {ip} {port}\"");
                    Disconnect();
                }
            }

            Console.Write("Введи имя: ");
            _userName = Console.ReadLine();
            _client = new TcpClient();
            try
            {
                _client.Connect(_host, _port); 
                _stream = _client.GetStream(); 

                string message = _userName;
                byte[] data = Encoding.UTF8.GetBytes(message ?? "Клиент");
                _stream.Write(data, 0, data.Length);
                
                Thread receiveThread = new Thread(ReceiveMessage); 
                receiveThread.Start();

                Console.WriteLine("Привет, {0}", _userName);
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }
        
        static void SendMessage()
        {
            Console.WriteLine("Вводи сообщения: ");

            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.UTF8.GetBytes(message ?? string.Empty);
                _stream.Write(data, 0, data.Length);
            }
        }
        
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; 
                    StringBuilder builder = new StringBuilder();
                    do
                    {
                        var bytes = _stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (_stream.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message);
                }
                catch
                {
                    Console.WriteLine("Подключение потеряно");
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            _stream?.Close();
            _client?.Close();
            Environment.Exit(0); 
        }
    }
}

