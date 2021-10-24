using System;
using System.Text;
using System.Threading;

namespace TcpChat
{
    class Program
    {
        private static Server _server;
        private static Thread _listenThread; 

        static void Main()
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            try
            {
                _server = new Server();
                _listenThread = new Thread(_server.Listen);
                _listenThread.Start(); 
            }
            catch (Exception ex)
            {
                _server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
