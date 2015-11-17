/** 
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
using Assets.Scripts.Interfaces;
using HeddokoLib.networking;


namespace Assets.Scripts.Communication
{
    /**
    * SocketClient class 
    * @brief SocketClient class: a socket connection to the windows Brainpack service
    * @note something 
    */

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
                mStreamWriter.Write(vMsg);
                mStreamWriter.Flush();
            }
            catch (IOException e)
            {
                UnityEngine.Debug.Log(e.Message);
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
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
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
        public void ReadServerSocket()
        {
            while (true)
            {
                if (mNetworkStream.DataAvailable)
                {
                    string result = mStreamReader.ReadToEnd();
                    byte[] vConverted = PacketSetting.Encoding.GetBytes(result);
                    HeddokoPacket vPacket = new HeddokoPacket(vConverted, 4);
                    string command = vPacket.Command;
                    PacketCommandRouter.Instance.Process(command, vPacket);
                    mSocketReady = false;
                }
                if (!mIsWorking)
                {
                    CloseSocket();
                    break;
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
                mWorkerThread.Start();
            }
        }
    }
}
