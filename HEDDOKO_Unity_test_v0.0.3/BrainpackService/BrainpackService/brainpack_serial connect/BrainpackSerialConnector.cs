using System;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Threading;
using BrainpackService.bluetooth_connector.BrainpackInterfaces;
using BrainpackService.Tools_and_Utilities;
using HeddokoLib.adt;

namespace BrainpackService.brainpack_serial_connect
{
    public class BrainpackSerialConnector : IBrainpackConnection
    {
        private Thread mWorkerThread;
        private bool mIsWorking { get; set; }
        //public FileStream dataFile;
        //public DataTable analysisData;
        private string mPortName;
        private static BrainpackSerialConnector sInstance;

        SerialPort mSerialport { get; set; } = new SerialPort();

        CircularQueue<string> OutboundBuffer { get; set; } = new CircularQueue<string>(1024, true);

        public static BrainpackSerialConnector Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new BrainpackSerialConnector();
                }
                return sInstance;
            }
        }

        public void Initialize(string vBrainpackName)
        {

            SerialPortOpen(vBrainpackName);


        }

        private void SerialPortOpen(string vNewPort)
        {
            try
            {
                if (vNewPort != mPortName)
                {
                    Stop();
                    mSerialport = new SerialPort();
                    mPortName = vNewPort;
                    mSerialport.PortName = mPortName;
                    mSerialport.NewLine = "\r\n";
                    mWorkerThread = new Thread(ThreadedFunction);
                    this.mSerialport.BaudRate = 115200;
                    Start();
                }
                else
                {
                    Start();
                }
               
            }
            catch (Exception e)
            {
                 BrainpackEventLogManager.InvokeEventLogError("Could not open serial port "+mSerialport+"\n Error:"+e.StackTrace+"\n"+e.ToString());
            }
           
           
        }

        public void ThreadedFunction()
        {

            while (true)
            {
                if (mSerialport.IsOpen)
                {
                    try
                    {
                        string line = mSerialport.ReadLine();
                        OutboundBuffer.Enqueue(line);
                    }
                    catch ( IOException vIoException)
                    {
                         BrainpackEventLogManager.InvokeEventLogError(vIoException +"\r\n"+vIoException.StackTrace);
                    }
                  
                }
                if (!mIsWorking)
                {
                    Stop();
                    break;
                }
            }
        }


        public void SendCommandToBrainpack(string vMsg)
        {
            mSerialport.Write(vMsg + "\r\n");
        }


        public void Start()
        {
            try
            { 
                mSerialport.Open();
            }
            catch (Exception e)
            {
                BrainpackEventLogManager.InvokeEventLogError(e.ToString()); 
            }

            if (!mIsWorking)
            { 
                mIsWorking = true;
                mWorkerThread.Start();
            }

        }


        /// <summary>
        /// Stop the connection
        /// </summary>
        public void Stop()
        {
            mIsWorking = false;
            OutboundBuffer.Clear();
            if (IsConnected)
            {
                mSerialport.Close();
            }
        }

        public bool IsConnected
        {
            get
            {
                if (mSerialport != null)
                {
                    return mSerialport.IsOpen;
                }
                return false;
            } 
        }

        public string GetNextFrame()
        {
            if (OutboundBuffer.Count == 0)
            {
                return "";
            }
            else
            {
                return OutboundBuffer.Dequeue();
            }
        }
    }
}
