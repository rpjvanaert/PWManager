using General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WPF_PWM.Classes
{
    public class Client
    {

        private TcpClient client;
        private NetworkStream stream;

        private byte[] buffer = new byte[1024];
        private byte[] totalBuffer = new byte[1024];
        private int totalBufferReceived = 0;

        private IDataWindow dataWindow;
        private ILoginWindow loginWindow;

        private Client() : this("localhost", 1310)
        {

        }

        public Client(string adress, int port)
        {
            this.client = new TcpClient();
            client.BeginConnect(adress, port, new AsyncCallback(OnConnect), null);
        }

        private static readonly Client self = new Client();

        public static Client GetInstance()
        {
            return self;
        }

        private void OnConnect(IAsyncResult ar)
        {
            this.client.EndConnect(ar);
            this.stream = this.client.GetStream();
            this.stream.BeginRead(this.buffer, 0, this.buffer.Length, new AsyncCallback(OnRead), null);
        }

        private void OnRead(IAsyncResult ar)
        {
            if (ar == null || (!ar.IsCompleted) || (!this.stream.CanRead))
            {
                return;
            }

            int receivedBytes = this.stream.EndRead(ar);

            if (totalBufferReceived + receivedBytes > 1024)
            {
                throw new OutOfMemoryException("Buffer too small");
            }
            Array.Copy(buffer, 0, totalBuffer, totalBufferReceived, receivedBytes);
            totalBufferReceived += receivedBytes;

            int expectedMessageLength = BitConverter.ToInt32(totalBuffer, 0);

            while (totalBufferReceived >= expectedMessageLength)
            {
                byte[] messageBytes = new byte[expectedMessageLength];
                Array.Copy(totalBuffer, 0, messageBytes, 0, expectedMessageLength);

                byte[] payloadbytes = new byte[BitConverter.ToInt32(messageBytes, 0) - 5];

                Array.Copy(messageBytes, 5, payloadbytes, 0, payloadbytes.Length);

                string identifier;
                bool isJson = DataParser.getJsonIdentifier(messageBytes, out identifier);
                if (isJson)
                {
                    switch (identifier)
                    {
                        case DataParser.LOGIN_RESPONSE:
                            if(DataParser.GetStatusResponse(payloadbytes) == "OK")
                            {
                                this.loginWindow.Message("Log-in succesful!");
                                this.loginWindow.Login(true);
                            }
                            else
                            {
                                this.loginWindow.Message("Log-in unsuccesful...");
                                this.loginWindow.Login(false);
                            }
                            break;
                        case DataParser.ADD_RESPONSE:
                            if (DataParser.GetStatusResponse(payloadbytes) == "OK")
                            {
                                this.dataWindow.Message("Add succesful!");
                            }
                            else
                            {
                                this.dataWindow.Message("Add unsuccesful...");
                            }
                            
                            break;

                        case DataParser.DELETE_RESPONSE:
                            if (DataParser.GetStatusResponse(payloadbytes) == "OK")
                            {
                                this.dataWindow.Message("Delete succesful!");
                            }
                            else
                            {
                                this.dataWindow.Message("Delete unsuccesful...");
                            }
                            break;

                        case DataParser.DATA_RESPONSE:
                            if (DataParser.GetStatusResponse(payloadbytes) == "OK")
                            {
                                this.dataWindow.GiveData(DataParser.GetData(payloadbytes));
                                this.dataWindow.Message("Refresh succesful!");
                            }
                            else
                            {
                                this.dataWindow.Message("Refresh unsuccesful...");
                            }
                            break;
                    }
                }
                totalBufferReceived -= expectedMessageLength;
                expectedMessageLength = BitConverter.ToInt32(totalBuffer, 0);
            }
            this.stream.BeginRead(this.buffer, 0, this.buffer.Length, new AsyncCallback(OnRead), null);
        }

        public void SetDataWindow(IDataWindow dataWindow)
        {
            this.dataWindow = dataWindow;
        }

        public void SetLoginWindow(ILoginWindow loginWindow)
        {
            this.loginWindow = loginWindow;
        }

        private void OnWrite(IAsyncResult ar)
        {
            this.stream.EndWrite(ar);
        }

        public void TryLogin(string username, string password)
        {
            byte[] message = DataParser.GetLoginMessage(username, password);
            this.stream.BeginWrite(message, 0, message.Length, new AsyncCallback(OnWrite), null);
        }

        public void RequestRefresh(string username, string password)
        {
            byte[] message = DataParser.GetDataMessage(username, password);
            this.stream.BeginWrite(message, 0, message.Length, new AsyncCallback(OnWrite), null);
        }

        public void AddRequest(string mUsername, string mPassword, LoginCredentials loginCredentials)
        {
            byte[] message = DataParser.GetAddMessage(mUsername, mPassword, loginCredentials);
            this.stream.BeginWrite(message, 0, message.Length, new AsyncCallback(OnWrite), null);
        }

        public void DeleteRequest(string username, string password, LoginCredentials deleteLogin)
        {
            byte[] message = DataParser.GetDeleteMessage(username, password, deleteLogin);
            this.stream.BeginWrite(message, 0, message.Length, new AsyncCallback(OnWrite), null);
        }
    }
}
