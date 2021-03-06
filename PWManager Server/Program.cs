﻿using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using General;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Server
{

    class Program
    {
        static void Main(string[] args)
        {
            Communication communication = new Communication(new TcpListener(IPAddress.Any, 1310));
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

        public Server(TcpClient tcpClient)
        {
            this.client = tcpClient;
            this.stream = this.client.GetStream();
            this.stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
            this.saveData = new SaveData(Directory.GetCurrentDirectory() + "/data");
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

                Array.Copy(totalBuffer, expectedMessageLength, totalBuffer, 0, (totalBufferReceived - expectedMessageLength));

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

        private async void HandleData(byte[] message)
        {
            //Console.WriteLine("got\n\r" + Encoding.ASCII.GetString(message));
            byte[] payloadbytes = new byte[BitConverter.ToInt32(message, 0) - 5];

            Array.Copy(message, 5, payloadbytes, 0, payloadbytes.Length);

            string id;
            bool isJson = DataParser.getJsonIdentifier(message, out id);
            if (isJson)
            {
                switch (id)
                {

                    case DataParser.LOGIN:

                        if (VerifyMessage(payloadbytes))
                        {
                            SendMessage(DataParser.GetLoginResponse("OK"));
                        }
                        else
                        {
                            SendMessage(DataParser.GetLoginResponse("Error"));
                        }
                        break;

                    case DataParser.DATA:
                        if (VerifyMessage(payloadbytes))
                        {
                            string username; string password;
                            DataParser.GetUsernamePassword(payloadbytes, out username, out password);
                            List<LoginCredentials> logins = saveData.GetLoginsUser(username, password);
                            SendMessage(DataParser.GetDataResponse("OK", logins));
                        }
                        else
                        {
                            SendMessage(DataParser.GetDataResponse("Error"));
                        }
                        break;

                    case DataParser.ADD:
                        if (VerifyMessage(payloadbytes))
                        {
                            LoginCredentials adding;
                            DataParser.GetAddition(payloadbytes, out adding);
                            SendMessage(DataParser.GetAddResponse("OK"));
                            string username; string password;
                            DataParser.GetUsernamePassword(payloadbytes, out username, out password);
                            await saveData.Add(username, password, adding);
                            await saveData.WriteDataJSON();
                        }
                        else
                        {
                            SendMessage(DataParser.GetAddResponse("Error"));
                        }
                        break;
                    case DataParser.DELETE:
                        if (VerifyMessage(payloadbytes))
                        {
                            
                            string username; string password;
                            DataParser.GetUsernamePassword(payloadbytes, out username, out password);
                            LoginCredentials deleting = DataParser.GetDeletion(payloadbytes);
                            Thread.Sleep(100);
                            if (saveData.Remove(username, password, deleting))
                            {
                                await saveData.WriteDataJSON();
                                SendMessage(DataParser.GetDeleteResponse("OK"));
                            } 
                            else
                            {
                                SendMessage(DataParser.GetDeleteResponse("Error"));
                            }
                            
                        }
                        break;

                    default:
                        Console.WriteLine($"Received Json with id {id}:\n{Encoding.ASCII.GetString(payloadbytes)}");
                        break;
                }
                dynamic json = JsonConvert.DeserializeObject(Encoding.ASCII.GetString(payloadbytes));

            }


        }

        private void SendMessage(byte[] message)
        {
            stream.BeginWrite(message, 0, message.Length, new AsyncCallback(OnWrite), null);
        }

        private bool VerifyMessage(byte[] PayloadMessage)
        {
            string username; string password;
            if (DataParser.GetUsernamePassword(PayloadMessage, out username, out password))
            {
                if (VerifyLogin(username, password))
                {
                    return true;
                }
            }
            return false;
        }

        private bool VerifyLogin(string username, string password)
        {
            //todo actual login
            return saveData.VerifyUser(username, password);
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
