
/** 
* @file SynchronousClient.cs
* @brief Contains the SynchronousClient class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/


using System.Collections.Generic;
using System.Linq; 
using HeddokoLib.networking;
using UnityEngine;

namespace Assets.Scripts.Communication
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Text;

    // State object for receiving data from remote device.
 

    public class SynchronousClient
    { 
        private Thread mWorkerThread;
   
        public SynchronousClient()
        {
            mWorkerThread = new Thread(ThreadWorker);
            mIsworking = true;
            mWorkerThread.Start();
        }

        public Queue<string> Requests = new Queue<string>(300);
        private static bool mReceivedMessage =true;
        private void ThreadWorker()
        {
            while (true)
            {
                if (!mIsworking)
                {
                    break;
                }
                if (Requests.Count == 0)
                {
                    continue;
                }

                if (mReceivedMessage == false)
                {
                    continue;
                }
                else
                { 
                    mReceivedMessage = false;
                    string vMsg = Requests.Dequeue();
                    StartClientAndSendData(vMsg);
                }

            }
        }
        /// <summary>
        /// Starts a client socket and sends the message data. 
        /// </summary>
        /// <param name="vMsg"></param>
        private static void StartClientAndSendData(string vMsg)
        {
            byte[] bytes = new byte[1024];
            mReceivedMessage = false;
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // The name of the 
                // remote device is "host.contoso.com".
                IPHostEntry vIpHostEntry = Dns.Resolve("localhost");
                IPAddress vIpAddress = vIpHostEntry.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
                IPEndPoint vRemoteEndPoint = new IPEndPoint(vIpAddress, 11000);
                // Create a TCP/IP  socket.
                Socket vSender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {

                    vSender.Connect(vRemoteEndPoint); 
                    // Encode the data string into a byte array.
                    byte[] msg = PacketSetting.Encoding.GetBytes(vMsg);

                    // Send the data through the socket.
                      vSender.Send(msg);

                    // Receive the response from the remote device.
                    vSender.Receive(bytes);
                
                    HeddokoPacket vHPacket = new HeddokoPacket(bytes, PacketSetting.PacketCommandSize);
                    PacketCommandRouter.Instance.Process(vSender, vHPacket);
                    
                    // Release the socket.
                    vSender.Shutdown(SocketShutdown.Both);
                    vSender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Debug.Log("ArgumentNullException  " +ane.ToString());
         SocketClient.WriteToLogFile("ArgumentNullException  " + ane.ToString());
                }
                catch (SocketException se)
                {
                    Debug.Log("SocketException  "+ se.ToString());
                    SocketClient.WriteToLogFile("ArgumentNullException  " + se.ToString());
                }
                catch (Exception e)
                {
                    Debug.Log("Unexpected exception " +e.ToString());
                    SocketClient.WriteToLogFile("ArgumentNullException  " + e.ToString());
                }

            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            mReceivedMessage = true;
        }
        private bool mIsworking;
        public void Stop()
        {
            mIsworking = false;
        }

 
    }
}
