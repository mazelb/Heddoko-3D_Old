
/** 
* @file SynchronousClient.cs
* @brief Contains the SynchronousClient class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/


using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts.Utils.DebugContext.logging;
//using Assets.Scripts.Utils.Debugging;
using HeddokoLib.networking;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Communication
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    // State object for receiving data from remote device.

    /// <summary>
    /// Synchrounous socket client 
    /// </summary>

    public class SynchronousClient
    {
        private Thread mWorkerThread;
        private const int sTimeout=10000; 

        public SynchronousClient()
        {
            mWorkerThread = new Thread(ThreadWorker);
            mIsworking = true;
            mWorkerThread.Start();
            string vLogMessage = "Synchronous Client instantiated, current timeout is set to " + sTimeout;
            DebugLogger.Instance.LogMessage(LogType.SocketClientSettings,vLogMessage);
        }

        public Queue<string> Requests = new Queue<string>(50);
        private static bool mReceivedMessage = true;

      
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
        public void StartClientAndSendData(string vMsg)
        {
            byte[] bytes = new byte[1024];
            mReceivedMessage = false;
            string vLogmessage = "";
            Stopwatch vStopwatch = new Stopwatch();
            vStopwatch.Start();
            // Connect to a remote device.
            try
            {
                IPHostEntry vIpHostEntry = Dns.Resolve("localhost");
                IPAddress vIpAddress = vIpHostEntry.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
                IPEndPoint vRemoteEndPoint = new IPEndPoint(vIpAddress, 11000);
                // Create a TCP/IP  socket.
                Socket vSender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                vSender.ReceiveTimeout = sTimeout;
                vSender.SendTimeout = sTimeout; 
             
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

                    // vMsg = PacketSetting.Encoding.GetString(bytes);
                }
                catch (TimeoutException vE)
                { 
                    vSender.Shutdown(SocketShutdown.Both);
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vE.Message); 
                    vMsg = "time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);

                    vSender.Close();
                }
                catch (ArgumentNullException vE)
                {
                    vMsg = "ArgumentNullException  " + vE;
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                    vMsg = "time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);

                    Debug.Log(vMsg);
                }
                catch (SocketException vE)
                {
                    
                    vMsg = "SocketException  "+ vE.ErrorCode + "\r\n"+ vE;
                    vMsg += vE.InnerException;
                    vSender.Close();
                    Debug.Log(vMsg);
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                    vMsg = "time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);

                }
                catch (Exception e)
                {
                    vMsg = "Unexpected exception " + e;
                    vSender.Shutdown(SocketShutdown.Both);
                    vSender.Close();
                    Debug.Log(vMsg);
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                    vMsg = "time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);

                }

            }
            catch (Exception e)
            {
                vMsg = "Unexpected exception " + e;
                DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                vMsg = "time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                Debug.Log(vMsg);
            }
            mReceivedMessage = true;
            vStopwatch.Stop();
            vMsg = "time taken from start send message to end " + vStopwatch.ElapsedMilliseconds + " ms";
        //    DebugLogger.Instance.LogMessage(LogType.SocketClientSettings, vMsg);


        }
        private bool mIsworking;
        public void Stop()
        {
            mIsworking = false;
        }


         
    }
}
