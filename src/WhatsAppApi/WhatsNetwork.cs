﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using WhatsAppApi.Helper;

namespace WhatsAppApi
{
    public class WhatsNetwork
    {
        private readonly int recvTimeout;
        private readonly string whatsHost;
        private readonly int whatsPort;

        //private string incomplete_message = "";
        private List<byte> incomplete_message = new List<byte>();
        private Socket socket;
        //private BinTreeNodeWriter binWriter;

        public WhatsNetwork(string whatsHost, int port, int timeout = 2000)
        //public WhatsNetwork(string whatsHost, int port, BinTreeNodeWriter writer, int timeout = 2000)
        {
            this.recvTimeout = timeout;
            this.whatsHost = whatsHost;
            this.whatsPort = port;
            //this.binWriter = writer;
            this.incomplete_message = new List<byte>();
        }

        public void Connect()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.Connect(this.whatsHost, this.whatsPort);
            this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, this.recvTimeout);
            
            //var tmpNetStream = new NetworkStream(this.socket);
            //this.streamReader = new StreamReader(tmpNetStream);
            //this.streamWriter = new StreamWriter(tmpNetStream);

            if (!this.socket.Connected)
                throw new ConnectionException("Cannot connect");
        }

        public void Disconenct()
        {
            if (this.socket.Connected)
                this.socket.Disconnect(true);
        }

        //public string ReadData()
        //{
        //    string buff = "";
        //    string ret = Socket_read(1024);
        //    if (ret != null)
        //    {
        //        buff = this.incomplete_message + ret;
        //        this.incomplete_message = "";
        //    }
        //    return buff;
        //}
        public byte[] ReadData()
        {
            List<byte> buff = new List<byte>();
            byte[] ret = Socket_read(1024);
            //if (ret != null)
            //{
            //    buff.AddRange(this.incomplete_message);
            //    buff.AddRange(ret);
            //    this.incomplete_message = new List<byte>();
            //}
            //return buff.ToArray();
            return ret;
        }

        public void SendData(string data)
        {
            Socket_send(data, data.Length, 0);
        }
        public void SendData(byte[] data)
        {
            Socket_send(data);
        }

        //public void SendNode(ProtocolTreeNode node)
        //{
        //    //this.DebugPrint(node.NodeString("SENT: "));
        //    this.SendData(this.binWriter.Write(node));
        //}

        //private string Socket_read(int length)
        //{
        //    if (!socket.Connected)
        //    {
        //        throw new System.IO.IOException("Disconnected");
        //    }

        //    var buff = new byte[length];
        //    int receiveLength = 0;
        //    try
        //    {
        //        receiveLength = socket.Receive(buff, 0, length, 0);
        //    }
        //    catch (SocketException excpt)
        //    {
        //        if (excpt.SocketErrorCode == SocketError.TimedOut)
        //        {
        //            Console.WriteLine("Socket-Timout");
        //            return null;
        //        }
        //        else
        //        {
        //            Console.WriteLine("Unbehandelter Fehler bei Sockerread: {0}", excpt);
        //            throw excpt;
        //        }
        //    }

        //    string tmpRet = this.sysEncoding.GetString(buff);
        //    return tmpRet;
        //}
        private byte[] Socket_read(int length)
        {
            if (!socket.Connected)
            {
                throw new ConnectionException();
            }

            var buff = new byte[length];
            int receiveLength = 0;
            do
            {
                try
                {
                    receiveLength = socket.Receive(buff, 0, length, 0);
                }
                catch (SocketException excpt)
                {
                    if (excpt.SocketErrorCode == SocketError.TimedOut)
                    {
                        Console.WriteLine("Socket-Timout");
                        //throw new ConnectionException("Timeout", excpt);
                        return null;
                    }
                    else
                    {
                        Console.WriteLine("Unbehandelter Fehler bei Sockerread: {0}", excpt);
                        throw new ConnectionException("error", excpt);
                    }
                }
            } while (receiveLength <= 0);

            byte[] tmpRet = new byte[receiveLength];
            if (receiveLength > 0)
                Buffer.BlockCopy(buff, 0, tmpRet, 0, receiveLength);
            return tmpRet;
        }

        private void Socket_send(string data, int length, int flags)
        {
            var tmpBytes = WhatsApp.SYSEncoding.GetBytes(data);
            this.socket.Send(tmpBytes);
        }
        private void Socket_send(byte[] data)
        {
            this.socket.Send(data);
        }

        public bool SocketStatus
        {
            get { return socket.Connected; }
        }
    }
}
