using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BrainpackService.Tools_and_Utilities;
using HeddokoLib.networking;
using HeddokoLib.utils;

namespace BrainpackService.BrainpackServer
{
    public class BrainpackServer
    { 
        private static Socket mServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private int mPortNumber = NetworkingReferences.ServerPort;
        private int mBacklog = 50;
        private static List<Socket> mClientsToServe = new List<Socket>(5);
        private int mTotalConnectedSockets;
        private AsyncCallback mWorkerCallback;
        public void SetupServer()
        {
            IPHostEntry vIpHostEntry = Dns.GetHostEntry("localhost");
            IPAddress vIpAddress = vIpHostEntry.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork); 
            IPEndPoint vLocalEndpoint = new IPEndPoint(vIpAddress, mPortNumber);

            mServerSocket.Bind(vLocalEndpoint);
            mServerSocket.Listen(mBacklog);
            mServerSocket.BeginAccept(new AsyncCallback(OnClientConnect), null);
        }

        void OnClientConnect(IAsyncResult vAR)
        {
            try
            {
                //Complete the begin accept async call 
                Socket vNewWorkerSocket = mServerSocket.EndAccept(vAR);
                mClientsToServe.Add(vNewWorkerSocket);
                //Let the worker soket do the procesing for the connected client
                WaitForData(mClientsToServe[mTotalConnectedSockets]);
                //increment the client count
                mTotalConnectedSockets++;

                mServerSocket.BeginAccept(new AsyncCallback(OnClientConnect), null);

            }
            catch (ObjectDisposedException)
            {
                BrainpackEventLogManager.InvokeEventLogMessage("Client Connection: socket has been closed");
            }
            catch (SocketException)
            {
                
            }
        }

        void WaitForData(Socket vSoc)
        {
            try
            {
                if (mWorkerCallback == null)
                {
                    //specify the callback function to be invoke if there is any 
                    //write activity
                    mWorkerCallback = new AsyncCallback(OnDataReceived);
                }
                HeddokoPacket vSocketPacket = new HeddokoPacket();
                vSocketPacket.Socket = vSoc;
                vSoc.BeginReceive(vSocketPacket.Payload,
                    0, vSocketPacket.Payload.Length, SocketFlags.None,
                    mWorkerCallback, vSocketPacket);
            }
            catch (SocketException)
            {
                
                throw;
            }
        }

        void OnDataReceived(IAsyncResult vAR)
        {
            try
            {
                HeddokoPacket vSocketData = (HeddokoPacket) vAR.AsyncState;
                int vBytesRead = 0;
                //complete BeginReceive() by EndReceive(), returning the number of bytes
                //written to the stream
                vBytesRead = vSocketData.Socket.EndReceive(vAR);
                if (vBytesRead > 0)
                {
                    StringBuilder vSb = new StringBuilder();
                    vSb.Append(PacketSetting.Encoding.GetString(vSocketData.Payload, 0, vBytesRead));
                    //there might be more data, so state the data received so far
                    string vUnwrappedString = vSb.ToString();
                    if (vUnwrappedString.IndexOf(PacketSetting.EndOfPacketDelim) > -1)
                    {
                        vSocketData.Command = HeddokoCommands.ExtractCommandFromBytes(0, 4, vSocketData.Payload); //extract the command out of the packet
                       
                        ServerCommandRouter.Instance.Process(vSocketData.Socket, vSocketData);  
                    }
                    else
                    {
                        WaitForData(vSocketData.Socket);
                    }
                } 
            }
            catch (Exception)
            {
                
                throw;
            }
        }
/*        private static  void AcceptCallbackConnection(IAsyncResult vAR)
        {
            Socket vSocket = (Socket) mServerSocket.EndAccept(vAR);
            mClientsToServe.Add(vSocket);
            mServerSocket.BeginReceive(mBuffer, 0, mBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            mServerSocket.BeginAccept(new AsyncCallback(AcceptCallbackConnection), null);
        }*/

  /*      private static void ReceiveCallback(IAsyncResult vAR)
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
        }*/

/*
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
*/

        public void ShutDown()
        {
            if (mServerSocket != null)
            {
                //send a message to all connected clients that server is shutting down

            }
        }

        public bool IsConnected()
        {
            if (mServerSocket != null)
            {
                return mServerSocket.Connected;
            }
            return false;
        }

        public void Send(Socket vSocket, string vResultPacket)
        {
            if (IsConnected() && vSocket.Connected)
            {
                try
                {
                    StringBuilder vSb = new StringBuilder();
                    byte[] vToSend = PacketSetting.Encoding.GetBytes(vResultPacket);
                    vSocket.Send(vToSend);
                }
                catch (SocketException e)
                {
                    
                }
              
            }
        
        }
    }
}
