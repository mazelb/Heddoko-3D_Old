﻿/** 
* @file SocketClient.cs
* @brief Contains the SocketClient clas
* @author Mohammed Haider (mohammed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using HeddokoLib.networking;
using HeddokoLib.utils;
using UnityEngine;


namespace Assets.Scripts.Communication
{
    /**
    * SocketClient class 
    * @brief SocketClient class: a socket connection to the windows Brainpack service
 
    */
    //TODO: need to check for forcible connection closes
    public class SocketClient
    {
        private TcpClient mSocket;
        NetworkStream mNetworkStream;
        StreamWriter mStreamWriter;
        StreamReader mStreamReader;
        private bool mSocketReady { get; set; }
        private bool mSocketOpen = false;
        private bool mIsWorking = false;
        private Thread mWorkerThread;
        private ISocketClientSetting mClientSocketSettings;
        public ISocketClientSetting ClientSocketSettings
        {
            get { return mClientSocketSettings; }
            set { mClientSocketSettings = value; }
        }
        /// <summary>
        /// Sends a message to the server with the provided message
        /// </summary>
        ///<param name="vMsg">The message to send to the server</param>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        /**
        * SendMessage(string vMsg)
        * @brief Sends a message to the server with the provided message
        * @param string vMessage The message to send to the server
        * @note Please not that this will throw an IOException if an I/O error occurs when attempting to write to the stream
        */
        public void SendMessage(string vMsg)
        {
            if (!mSocketReady)
            {
                OpenSocket();
            }
            try
            {
                mStreamWriter.WriteLine(vMsg);
                mStreamWriter.Flush();
            }
            catch (IOException e)
            {
                string vMessage =
                    "A connection to the Heddoko service failed. Please ensure that the service is operational and try again." +
                    "\n ---Beginning of Stacktrace---" + e.StackTrace + "---End of Stacktrace---" +
                    "\n ---Beginning of BaseException---" + e.GetBaseException() + "---End of GetBaseException---" +
                    "\n ---Beginning of BaseException Stack trace " + e.GetBaseException().StackTrace +
                    "---End of GetBaseException stacktrace---" +
                    "\n ----Beginning of InnerException" + e.InnerException + "---End of InnerException---" +
                    "\n ----Beginning of InnerException StackTrace" + e.InnerException.StackTrace +
                    "---End of InnerException StackTrace---";
                HeddokoPacket vPacket = new HeddokoPacket(HeddokoCommands.ClientError, vMessage);
                PacketCommandRouter.Instance.Process(this, vPacket);
            }
            catch (NullReferenceException vNullReferenceException)
            {
                //this error occurs if the mstreamwriter hasn't been instantiated and most likely will occur because the socket failed
                //to open a connection to the brainpack service. It was handled before
                //todo: check if there are any other cases where this exception can be raised

            }
            catch (Exception e)
            {
                string vMessage =
                       "A connection to the Heddoko service failed. Please ensure that the service is operational and try again." +
                       "\n ---Beginning of Stacktrace---" + e.StackTrace + "---End of Stacktrace---" +
                       "\n ---Beginning of BaseException---" + e.GetBaseException() + "---End of GetBaseException---" +
                       "\n ---Beginning of BaseException Stack trace " + e.GetBaseException().StackTrace +
                       "---End of GetBaseException stacktrace---" +
                       "\n ----Beginning of InnerException" + e.InnerException + "---End of InnerException---" +
                       "\n ----Beginning of InnerException StackTrace" + e.InnerException.StackTrace +
                       "---End of InnerException StackTrace---";
                UnityEngine.Debug.Log(vMessage);
            }
        }

        /// <summary>
        /// Attempt to open the client socket to the windows service
        /// </summary> 
        /// <exception cref="InvalidOperationException"> The TcpClient is not connected to a remote host.</exception>
        /// <exception cref="ObjectDisposedException">The TcpClient has been closed.</exception>      
         /**
        * OpenSocket( )
        * @brief Sends a message to the server with the provided message 
        * @note Please not that this will throw an exception if TcpClient is not connected to a remote host 
        * or if the TcpClient has been closed
        */
        public void OpenSocket()
        {
            try
            {
                mSocket = new TcpClient(ClientSocketSettings.ConnectionName, ClientSocketSettings.Port);
                mNetworkStream = mSocket.GetStream();
                mStreamWriter = new StreamWriter(mNetworkStream);
                mStreamReader = new StreamReader(mNetworkStream);
                mSocketReady = true;
            }
            catch (SocketException)
            {
                HeddokoPacket vPacket = new HeddokoPacket(HeddokoCommands.ClientError, "A connection to the Heddoko service failed. Please ensure that " +
                                                                                       "the service is operational and try again.");
                PacketCommandRouter.Instance.Process(this, vPacket);

            }
            catch (Exception e)
            {
                /*                string vMessage =
                                         "A connection to the Heddoko service failed. Please ensure that the service is operational and try again." +
                                         "\n ---Beginning of Stacktrace---" + e.StackTrace + "---End of Stacktrace---" +
                                         "\n ---Beginning of BaseException---" + e.GetBaseException() + "---End of GetBaseException---" +
                                         "\n ---Beginning of BaseException Stack trace " + e.GetBaseException().StackTrace +
                                         "---End of GetBaseException stacktrace---" +
                                         "\n ----Beginning of InnerException" + e.InnerException + "---End of InnerException---" +
                                         "\n ----Beginning of InnerException StackTrace" + e.InnerException.StackTrace +
                                         "---End of InnerException StackTrace---";
                                UnityEngine.Debug.Log(vMessage);*/
            }
        }

        /// <summary>
        /// Threaded function that will receive data from the network stream if avaiable, routes the commands over 
        /// to the CommandRouter singleton. Closes the thread if no longer working
        /// </summary>
        /**
       * ReadServerSocket( )
       * @brief Threaded function that will receive data from the network stream if avaiable, routes the commands over 
       * to the CommandRouter singleton. Closes the thread if no longer working
       *  
       */
        int resetClientSocket = -1;
        public void ReadServerSocket()
        {
            resetClientSocket++;
            string debugcommand = "";
            while (true)
            {
                
                try
                {
                    if (mNetworkStream.DataAvailable)
                    {
                        string result = mStreamReader.ReadLine(); 
                        if (!string.IsNullOrEmpty(result) && result.Contains("<EOL>"))
                        { 
                            byte[] vConverted = PacketSetting.Encoding.GetBytes(result);
                            HeddokoPacket vPacket = new HeddokoPacket(vConverted, 4);
                            string command = vPacket.Command;
 
                            PacketCommandRouter.Instance.Process(command, vPacket); 
                        }
                        mSocketReady = false;
                    }
                    if (!mIsWorking)
                    {
                        CloseSocket();
                        break;
                    }
                }
                catch (Exception e)
                {
                    string vMessage ="Internal failure with  "+
                       "  command" + debugcommand + ". Reset client times: "+ resetClientSocket+  " at Current time "+ DateTime.Now + 
                       "\n ---Beginning of Stacktrace---" + e.StackTrace + "---End of Stacktrace---" +
                       "\n ---Beginning of BaseException---" + e.GetBaseException() + "---End of GetBaseException---" +
                       "\n ---Beginning of BaseException Stack trace " + e.GetBaseException().StackTrace +
                       "---End of GetBaseException stacktrace---" +
                       "\n ----Beginning of InnerException" + e.InnerException + "---End of InnerException---" +
                       "\n ----Beginning of InnerException StackTrace" + e.InnerException.StackTrace +
                       "---End of InnerException StackTrace---";
                    UnityEngine.Debug.Log(vMessage);
                }

            }

        }

        /// <summary>
        /// Closes the socket and releases the current resources
        /// </summary>
        /**
       * ReadServerSocket( )
       * @brief Closes the socket and releases the current resources
       *  
       */
        public void CloseSocket()
        {
            if (!mSocketReady)
            {
                return;
            }
            mStreamWriter.Close();
            mStreamReader.Close();
            mSocket.Close();
            mSocketReady = false;
        }
        /// <summary>
        /// Stops the current thread
        /// </summary>
        /**
       * Stop( )
       * @brief Stops the current thread
       *  
       */
        public void Stop()
        {
            mIsWorking = false;
        }

        /// <summary>
        /// Initializes the current SocketClient with the given socket client settings
        /// </summary>
        /// <param name="vSettings">ISocketClientSetting vSettings: socket client settings to initialize the current socket with </param>
        /**
        *Initialize(ISocketClientSetting vSettings)
        * @brief Initializes the current SocketClient with the given socket client
        * @param ISocketClientSetting vSettings: socket client settings to initialize the current socket with 
        */
        public void Initialize(ISocketClientSetting vSettings)
        {
            ClientSocketSettings = vSettings;
            mWorkerThread = new Thread(ReadServerSocket);
            OpenSocket();
        }
        /// <summary>
        /// Starts the current worker thread
        /// </summary> 
        /**
        * StartWorking()
        * @brief  Starts the current worker thread 
        */
        public void StartWorking()
        {
            if (mWorkerThread != null)
            {
                mIsWorking = true;
                mWorkerThread.Start();
            }
        }
    }
}