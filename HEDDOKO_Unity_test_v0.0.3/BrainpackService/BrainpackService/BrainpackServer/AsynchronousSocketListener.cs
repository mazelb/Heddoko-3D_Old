﻿using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BrainpackService.Tools_and_Utilities;
using HeddokoLib.networking;


namespace BrainpackService.BrainpackServer
{

    public class StateObject
    {
        // Client  socket.
        public Socket mClientSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] Buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder mStringBuilder = new StringBuilder();
    }
    public class AsynchronousSocketListener
    {
        //thread signal
        public static ManualResetEvent mCompleted = new ManualResetEvent(false);
        private static bool mServiceStopped = true;
        private static object mServiceObjectLock = new object();

        public static void StartListening()
        {
            lock (mServiceObjectLock)
            {
                mServiceStopped = false;
            }
            //buffer for incoming datata
            byte[] vBytes = new byte[1024];
            //establish the local endpoint for the socket
            //the dns name of the computer
            //running the listener 
            IPHostEntry vIpHostEntry = Dns.GetHostEntry("localhost");
            IPAddress vIpAddress = vIpHostEntry.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);//.AddressList[0];
            IPEndPoint vLocalEndpoint = new IPEndPoint(vIpAddress, 11000);

            //create a tcp/ip socket
            Socket vListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //bind the socket to the local endpoint and listen for incoming calls
            try
            {
                vListener.Bind(vLocalEndpoint);
                vListener.Listen(100);
                while (true)
                {
                    mCompleted.Reset();
                    vListener.BeginAccept(new AsyncCallback(AcceptCallback), vListener); //receive data 
                    mCompleted.WaitOne();
                    if (mServiceStopped)
                    {

                        break;
                    }
                }
            }
            catch (Exception e)
            {
                BrainpackEventLogManager.InvokeNetworkingException(e.StackTrace);
            }

        }

        public static void AcceptCallback(IAsyncResult vAR)
        {
          
            Socket vListener = (Socket)vAR.AsyncState;
            Socket vHandler = vListener.EndAccept(vAR);

            //create the state object

            StateObject vState = new StateObject();
            vState.mClientSocket = vHandler;
            vHandler.BeginReceive(vState.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), vState);
            mCompleted.Set();
        }

        public static void StopService()

        {
            mCompleted.Set();
            lock (mServiceObjectLock)
            {
                mServiceStopped = true;
            }
        }
        public static void ReadCallback(IAsyncResult vAr)
        {
            string vContent = string.Empty;
            //retrieve the state object and the handler socket
            //from the asynchronous state object
            StateObject vState = (StateObject)vAr.AsyncState;
            Socket vHander = vState.mClientSocket;

            //read the data from the client socket
            int bytesRead = vHander.EndReceive(vAr);
            if (bytesRead > 0)
            {
                //there might be more data, so state the data received so far
                vState.mStringBuilder.Append(PacketSetting.Encoding.GetString(vState.Buffer, 0, bytesRead));
                //check for EOF tag, if not there read more data
                vContent = vState.mStringBuilder.ToString();
                if (vContent.IndexOf(PacketSetting.EndOfPacketDelim) > -1)
                {
                    byte[] vData = vState.Buffer;
                    //first 4 bytes are the command codes
                    HeddokoPacket vHPacket = new HeddokoPacket(vData, PacketSetting.PacketCommandSize);
                    ServerCommandRouter.Instance.Process(vHander, vHPacket);
                    //all data has been read from the client. redistribute it// place commands here
                    //Send(vHander, vContent);
                }
                else
                {
                    //not all data received. get more
                    vHander.BeginReceive(vState.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, vState);
                }
            }
        }

        public static void Send(Socket vHandler, string vData)
        {
            //convert the string data to byte data using ASCII encoding
            byte[] vByteData = PacketSetting.Encoding.GetBytes(vData);
            //begin sending the data to the remote device
            vHandler.BeginSend(vByteData, 0, vByteData.Length, 0, new AsyncCallback(SendCallback), vHandler);
        }

        public static void Send(Socket vHandler, byte[] vByteData)
        {
            vHandler.BeginSend(vByteData, 0, vByteData.Length, 0, new AsyncCallback(SendCallback), vHandler);
        }

        private static void SendCallback(IAsyncResult vAr)
        {
            try
            {
                //retrieve the socket from the state object
                Socket vHandler = (Socket)vAr.AsyncState;

                //complete sending the data to the remote device
                int vBytesSent = vHandler.EndSend(vAr);


                // vHandler.Shutdown(SocketShutdown.Both);
                vHandler.Shutdown(SocketShutdown.Send);
                vHandler.Close();
                //  CloseConnectionToSocket(vHandler); 
            }
            catch (Exception e)
            {
                BrainpackEventLogManager.InvokeNetworkingException(e.StackTrace);
            }
        }

        private static void CloseConnectionToSocket(Socket vHandler)
        {

        }
    }
}
