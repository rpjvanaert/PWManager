using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Text;
using Server;

namespace Server
{
    class Communication
    {
        private TcpListener listener;
        private List<Server> clients;

        public Communication(TcpListener listener)
        {
            this.listener = listener;
            this.clients = new List<Server>();
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine($"==========================================================================\n" +
                $"\tstarted accepting clients at {DateTime.Now}\n" +
                $"==========================================================================");
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
        }

        private void OnConnect(IAsyncResult ar)
        {
            var tcpClient = listener.EndAcceptTcpClient(ar);
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");
            clients.Add(new Server(tcpClient));
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
        }

        internal void Disconnect(Server client)
        {
            clients.Remove(client);
        }
    }
}
