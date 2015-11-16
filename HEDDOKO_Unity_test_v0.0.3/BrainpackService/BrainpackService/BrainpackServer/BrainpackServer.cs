using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BrainpackService.Tools_and_Utilities;
using HeddokoLib.networking;

namespace BrainpackService.BrainpackServer
{
    public class BrainpackServer
    {
        private static byte[] mBuffer = new byte[256];
        private static Socket mServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private int mPortNumber = NetworkingReferences.ServerPort;
        private int mBacklog = 5;
        private static List<Socket> mClientsToServe = new List<Socket>(5); 
        public void SetupServer()
        {
            IPHostEntry vIpHostEntry = Dns.GetHostEntry("localhost");
            IPAddress vIpAddress = vIpHostEntry.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork); 
            IPEndPoint vLocalEndpoint = new IPEndPoint(vIpAddress, mPortNumber);

            mServerSocket.Bind(vLocalEndpoint);
            mServerSocket.Listen(mBacklog);
            mServerSocket.BeginAccept(new AsyncCallback(AcceptCallbackConnection), null);
        }

        private static  void AcceptCallbackConnection(IAsyncResult vAR)
        {
            Socket vSocket = (Socket) mServerSocket.EndAccept(vAR);
            mClientsToServe.Add(vSocket);
            mServerSocket.BeginReceive(mBuffer, 0, mBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            mServerSocket.BeginAccept(new AsyncCallback(AcceptCallbackConnection), null);
        }

        private static void ReceiveCallback(IAsyncResult vAR)
        { 
            //todo:  threading here,
            Socket vSocket = (Socket) vAR.AsyncState;

            //first we need to get the number of data received
            int vReceived = vSocket.EndReceive(vAR);
            //since we're not specifying any length parameters in the data being sent, trim the data received
            byte[] mDataBuff = new byte[vReceived];
            Array.Copy(mBuffer,mDataBuff, vReceived);

            string mText = PacketSetting.Encoding.GetString(mDataBuff);

            //todo Place commands here
            if (mText.ToLower() == "get time")
            {
                SendData(vSocket,DateTime.Now.ToLongDateString());
            }
            else
            {
                SendData(vSocket,"Invalid request");
            }
            mServerSocket.BeginReceive(mBuffer, 0, mBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            mServerSocket.BeginAccept(new AsyncCallback(AcceptCallbackConnection), null);
        }

        public static void SendData(Socket vSocket ,string text)
        {
            byte[] vData = PacketSetting.Encoding.GetBytes(text);
            vSocket.BeginSend(vData, 0, vData.Length, SocketFlags.None, new AsyncCallback(SendCallback), vSocket);
            vSocket.BeginReceive(mBuffer, 0, mBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback),
                vSocket); 
        }
        private static void SendCallback(IAsyncResult vAR)
        {
            Socket vSocket = (Socket) vAR.AsyncState;
            vSocket.EndSend(vAR);
        }

        private static void ProcessRequest()
        {
            
        }
    }
}
