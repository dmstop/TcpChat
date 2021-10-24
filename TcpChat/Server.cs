using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TcpChat
{
    public class Server
    {
        private static TcpListener _tcpListener; 
        private readonly List<Client> _clients = new List<Client>(); 

        protected internal void AddConnection(Client clientObject)
        {
            _clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            Client client = _clients.FirstOrDefault(c => c.Id == id);
            
            if (client != null)
                _clients.Remove(client);
        }
        
        protected internal void Listen()
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Any, 1337);
                _tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ждем подключения...");

                while (true)
                {
                    TcpClient tcpClient = _tcpListener.AcceptTcpClient();

                    Client clientObject = new Client(tcpClient, this);
                    Thread clientThread = new Thread(clientObject.Process);
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        
        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            foreach (var client in _clients.Where(client => client.Id != id))
            {
                client.Stream.Write(data, 0, data.Length);
            }
        }
        
        protected internal void Disconnect()
        {
            _tcpListener.Stop(); 

            foreach (var client in _clients)
            {
                client.Close(); 
            }
            Environment.Exit(0); 
        }
    }
}

