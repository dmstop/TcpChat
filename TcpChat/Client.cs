using System;
using System.Net.Sockets;
using System.Text;

namespace TcpChat
{
    public class Client
    {
        public readonly Server Server;
        protected internal string Id { get; }
        protected internal NetworkStream Stream { get; private set; }
        private string _userName;
        private readonly TcpClient _client;
        

        public Client(TcpClient tcpClient, Server serverObject)
        {
            Id = Guid.NewGuid().ToString();
            _client = tcpClient;
            Server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = _client.GetStream();
                
                string message = GetMessage();
                _userName = message;

                message = _userName + " вошел в чат";
                
                Server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = $"{_userName}: {message}";
                        Console.WriteLine(message);
                        Server.BroadcastMessage(message, this.Id);
                    }
                    catch
                    {
                        message = $"{_userName} покинул чат";
                        Console.WriteLine(message);
                        Server.BroadcastMessage(message, Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Server.RemoveConnection(Id);
                Close();
            }
        }

        private string GetMessage()
        {
            byte[] data = new byte[64]; 
            StringBuilder builder = new StringBuilder();
            do
            {
                var bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        protected internal void Close()
        {
            Stream?.Close();
            _client?.Close();
        }
    }
}
