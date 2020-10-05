using Newtonsoft.Json;
using Server;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PWManager_Server
{

    class Program
    {
        static void Main(string[] args)
        {
            Communication communication = new Communication(new TcpListener(IPAddress.Any, 102020));
            communication.Start();

            while (true) { }
        }
    }

    class Server
    {
        private TcpClient client;
        private NetworkStream stream;
        private byte[] buffer = new byte[1024];
        private byte[] totalBuffer = new byte[1024];
        private int totalBufferReceived = 0;
        private SaveData saveData;
        private DateTime sessionStart;
        
        public Server(TcpClient tcpClient)
        {
            this.sessionStart = DateTime.Now;
            this.client = tcpClient;
            this.stream = this.client.GetStream();
            this.stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        private void OnRead(IAsyncResult ar)
        {
            int receivedBytes = this.stream.EndRead(ar);

            if (totalBufferReceived + receivedBytes > 1024)
            {
                throw new OutOfMemoryException("Buffer is too small.");
            }
            Array.Copy(buffer, 0, totalBuffer, totalBufferReceived, receivedBytes);
            totalBufferReceived += receivedBytes;

            int expectedMessageLength = BitConverter.ToInt32(totalBuffer, 0);
            while (totalBufferReceived >= expectedMessageLength)
            {
                byte[] messageBytes = new byte[expectedMessageLength];
                Array.Copy(totalBuffer, 0, messageBytes, 0, expectedMessageLength);
                HandleData(messageBytes);

                Array.Copy(totalBuffer, expectedMessageLength, totalBuffer, 0, (totalBufferReceived - expectedMessageLength)); //maybe unsafe idk 

                totalBufferReceived -= expectedMessageLength;
                expectedMessageLength = BitConverter.ToInt32(totalBuffer, 0);
                if (expectedMessageLength <= 5)
                {
                    break;
                }
            }

            this.stream.BeginRead(this.buffer, 0, this.buffer.Length, new AsyncCallback(OnRead), null);
        }

        private void OnWrite(IAsyncResult ar)
        {
            this.stream.EndWrite(ar);
        }

        private void HandleData(byte[] message)
        {
            
            byte[] payloadbytes = new byte[BitConverter.ToInt32(message, 0) - 5];

            Array.Copy(message, 5, payloadbytes, 0, payloadbytes.Length);

            string identifier;
            bool isJson = DataParser.getJsonIdentifier(message, out identifier);
            if (isJson)
            {
                switch (identifier)
                {
                    
                }
                Array.Copy(message, 5, payloadbytes, 0, message.Length - 5);
                dynamic json = JsonConvert.DeserializeObject(Encoding.ASCII.GetString(payloadbytes));
                saveData.WriteDataJSON(Encoding.ASCII.GetString(payloadbytes));

            }
            else if (DataParser.isRawData(message))
            {
                Console.WriteLine(BitConverter.ToString(message));
                saveData.WriteDataRAW(ByteArrayToString(message));
            }


        }

        private bool verifyLogin(string username, string password)
        {
            return username == password;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
